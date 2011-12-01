using System.Collections;
using System.Collections.Generic;
using EchoNest;
using EchoNest.Artist;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Common;
using System.Linq;

namespace Torshify.Radio.EchoNest.Style
{
    public class StylesToArtistEnumerator : IEnumerator<IEnumerable<RadioTrack>>
    {
        #region Fields

        private Queue<ArtistBucketItem> _artistsToLookFor;
        private IEnumerable<RadioTrack> _currentArtistTracks;
        private IRadio _radio;
        private IEnumerable<TermModel> _terms;

        #endregion Fields

        #region Constructors

        public StylesToArtistEnumerator()
        {
            NumberOfTracksPerArtist = 2;
        }

        #endregion Constructors

        #region Properties

        public int Count
        {
            get; set;
        }

        public IEnumerable<RadioTrack> Current
        {
            get { return _currentArtistTracks; }
        }

        public int NumberOfTracksPerArtist
        {
            get; set;
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
            _terms = null;

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

        public void Initialize(IEnumerable<TermModel> terms, IRadio radio)
        {
            _terms = terms;
            _radio = radio;

            Count = 10;
        }

        public bool MoveNext()
        {
            if (_artistsToLookFor == null || _artistsToLookFor.Count == 0)
            {
                _artistsToLookFor = SearchArtistMatchingMoods();
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

        private Queue<ArtistBucketItem> SearchArtistMatchingMoods()
        {
            using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
            {
                var searchArgument = new SearchArgument
                {
                    Results = Count,
                    Start = Start
                };

                foreach (var termModel in _terms)
                {
                    var term = searchArgument.Styles.Add(termModel.Name);

                    if (!DoubleUtilities.AreClose(termModel.Boost, 1.0))
                    {
                        term.Boost(termModel.Boost);
                    }

                    if (termModel.Require)
                    {
                        term.Require();
                    }

                    if (termModel.Ban)
                    {
                        term.Ban();
                    }
                }

                var result = session.Query<Search>().Execute(searchArgument);

                if (result != null && result.Status.Code == ResponseCode.Success)
                {
                    Start += result.Artists.Count;
                    return new Queue<ArtistBucketItem>(result.Artists);
                }

                return new Queue<ArtistBucketItem>();
            }
        }

        #endregion Methods
    }
}