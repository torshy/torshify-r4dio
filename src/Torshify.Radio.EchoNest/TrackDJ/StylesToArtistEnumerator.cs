using System;
using System.Collections;
using System.Collections.Generic;
using EchoNest;
using EchoNest.Song;
using Torshify.Radio.Framework;
using System.Linq;

namespace Torshify.Radio.EchoNest.TrackDJ
{
    public class TrackDJSongEnumerator : IEnumerator<IEnumerable<RadioTrack>>
    {
        #region Fields

        private Queue<SongBucketItem> _songsToLookFor;
        private IEnumerable<RadioTrack> _currentArtistTracks;
        private IRadio _radio;
        private SearchArgument _searchArgument;
        #endregion Fields

        #region Properties

        public int Count
        {
            get;
            set;
        }

        public IEnumerable<RadioTrack> Current
        {
            get { return _currentArtistTracks; }
        }

        public int Start
        {
            get;
            set;
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            _songsToLookFor = null;
            _radio = null;

            Start = 0;
        }

        public IEnumerable<RadioTrack> DoIt()
        {
            if (MoveNext())
            {
                return Current;
            }

            return new RadioTrack[0];
        }

        public void Initialize(SearchArgument searchArgument, IRadio radio)
        {
            _radio = radio;
            _searchArgument = searchArgument;

            Count = 50;
        }

        public bool MoveNext()
        {
            if (_songsToLookFor == null || _songsToLookFor.Count == 0)
            {
                _songsToLookFor = GetSongsToLookFor();
            }

            if (_songsToLookFor.Count > 0)
            {
                var songToLookFor = _songsToLookFor.Dequeue();
                var tracks = _radio.GetTracksByArtist(songToLookFor.ArtistName, 0, 100);

                _currentArtistTracks = tracks.Where(t => t.Name.Equals(songToLookFor.Title));

                if (!_currentArtistTracks.Any())
                {
                    return MoveNext();
                }

                return true;
            }

            return false;
        }

        public void Reset()
        {
            _songsToLookFor = null;
        }

        private Queue<SongBucketItem> GetSongsToLookFor()
        {
            using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
            {
                _searchArgument.Start = Start;
                _searchArgument.Results = Count;

                var result = session.Query<Search>().Execute(_searchArgument);

                if (result != null && result.Status.Code == ResponseCode.Success)
                {
                    Start += result.Songs.Count;
                    return new Queue<SongBucketItem>(result.Songs.OrderBy(a => Guid.NewGuid()));
                }

                return new Queue<SongBucketItem>();
            }
        }

        #endregion Methods
    }
}