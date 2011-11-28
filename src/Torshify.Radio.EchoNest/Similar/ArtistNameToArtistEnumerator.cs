using System.Collections;
using System.Collections.Generic;
using System.Linq;

using EchoNest.Artist;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Similar
{
    public class ArtistNameToArtistEnumerator : IEnumerator<IEnumerable<IRadioTrack>>
    {
        #region Fields

        private IEnumerable<ArtistBucketItem> _artistNames;
        private Queue<ArtistBucketItem> _artistsToLookFor;
        private IEnumerable<IRadioTrack> _currentArtistTracks;
        private IRadio _radio;

        #endregion Fields

        #region Constructors

        public ArtistNameToArtistEnumerator()
        {
            NumberOfTracksPerArtist = 2;
        }

        #endregion Constructors

        #region Properties

        public int Count
        {
            get; set;
        }

        public IEnumerable<IRadioTrack> Current
        {
            get { return _currentArtistTracks; }
        }

        public int NumberOfTracksPerArtist
        {
            get;
            set;
        }

        public int Start
        {
            get; set;
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            _artistsToLookFor = null;
            _radio = null;
            _artistNames = null;

            Start = 0;
        }

        public IEnumerable<IRadioTrack> DoIt()
        {
            if (MoveNext())
            {
                return Current;
            }

            return new IRadioTrack[0];
        }

        public void Initialize(IEnumerable<ArtistBucketItem> artistNames, IRadio radio)
        {
            _artistNames = artistNames;
            _artistsToLookFor = new Queue<ArtistBucketItem>(artistNames);
            _radio = radio;

            Count = 10;
            Start = _artistNames.Count();
        }

        public bool MoveNext()
        {
            if (_artistsToLookFor == null || _artistsToLookFor.Count == 0)
            {
                _artistsToLookFor = Search();
            }

            if (_artistsToLookFor.Count > 0)
            {
                var artistToLookFor = _artistsToLookFor.Dequeue();
                _currentArtistTracks = _radio.GetTracksByArtist(artistToLookFor.Name, 0, NumberOfTracksPerArtist);

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
            _artistsToLookFor = null;
        }

        private Queue<ArtistBucketItem> Search()
        {
            //using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
            //{
            //    var searchArgument = new SearchArgument
            //    {
            //        Results = Count,
            //        Start = Start
            //    };

            //    var result = session.Query<Search>().Execute(searchArgument);

            //    if (result != null && result.Status.Code == ResponseCode.Success)
            //    {
            //        Start += result.Artists.Count;
            //        return new Queue<ArtistBucketItem>(result.Artists);
            //    }

            //    return new Queue<ArtistBucketItem>();
            //}

            return new Queue<ArtistBucketItem>();
        }

        #endregion Methods
    }
}