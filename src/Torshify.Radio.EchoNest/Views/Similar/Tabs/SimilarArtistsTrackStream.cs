using System.Collections;
using System.Collections.Generic;
using Torshify.Radio.Framework;
using System.Linq;

namespace Torshify.Radio.EchoNest.Views.Similar.Tabs
{
    public class SimilarArtistsTrackStream : ITrackStream
    {
        #region Fields

        private readonly IRadio _radio;
        private readonly IEnumerable<string> _similarArtists;
        private readonly IEnumerator<string> _similarArtistsEnumerator;

        private IEnumerable<Track> _currentTrackList;

        #endregion Fields

        #region Constructors

        public SimilarArtistsTrackStream(IRadio radio, IEnumerable<string> similarArtists)
        {
            _radio = radio;
            _similarArtists = similarArtists;
            _similarArtistsEnumerator =_similarArtists.GetEnumerator();

            _currentTrackList = new Track[0];
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<Track> Current
        {
            get { return _currentTrackList; }
        }

        public bool SupportsTrackSkipping
        {
            get { return true; }
        }

        public string Description
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
            _similarArtistsEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            if (_similarArtistsEnumerator.MoveNext())
            {
                _currentTrackList = _radio.GetTracksByName(_similarArtistsEnumerator.Current);

                if (!_currentTrackList.Any())
                {
                    return MoveNext();
                }

                return true;
            }

            _currentTrackList = new Track[0];
            return false;
        }

        public void Reset()
        {
            _similarArtistsEnumerator.Reset();
        }

        #endregion Methods
    }
}