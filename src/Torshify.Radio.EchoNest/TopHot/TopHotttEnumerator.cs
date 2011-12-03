using System;
using System.Collections;
using System.Collections.Generic;
using EchoNest;
using EchoNest.Artist;
using Torshify.Radio.Framework;
using System.Linq;

namespace Torshify.Radio.EchoNest.TopHot
{
    public class TopHotttEnumerator : IEnumerator<IEnumerable<RadioTrack>>
    {
        #region Fields

        private Queue<ArtistBucketItem> _artistsToLookFor;
        private IEnumerable<RadioTrack> _currentArtistTracks;
        private IRadioStationContext _context;

        #endregion Fields

        #region Constructors

        public TopHotttEnumerator(IRadioStationContext context)
        {
            _context = context;
            NumberOfTracksPerArtist = 2;
            Count = 10;
        }

        #endregion Constructors

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

        public int NumberOfTracksPerArtist
        {
            get;
            set;
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
            _artistsToLookFor = null;
            _context = null;

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

        public bool MoveNext()
        {
            if (_artistsToLookFor == null || _artistsToLookFor.Count == 0)
            {
                _artistsToLookFor = FetchNext();
            }

            if (_artistsToLookFor.Count > 0)
            {
                var artistToLookFor = _artistsToLookFor.Dequeue();
                _currentArtistTracks = _context.Radio.GetTracksByArtist(artistToLookFor.Name, 0, NumberOfTracksPerArtist);

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
            Start = 0;
        }

        private Queue<ArtistBucketItem> FetchNext()
        {
            try
            {
                using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
                {
                    var topHotttResponse = session.Query<TopHottt>().Execute(Count, Start);

                    if (topHotttResponse.Status.Code == ResponseCode.Success && topHotttResponse.Artists != null)
                    {
                        Start += topHotttResponse.Artists.Count;
                        return new Queue<ArtistBucketItem>(topHotttResponse.Artists.OrderBy(a => Guid.NewGuid()));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new Queue<ArtistBucketItem>();
        }

        #endregion Methods
    }
}