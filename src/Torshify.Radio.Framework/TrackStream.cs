using System.Collections;
using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public class TrackStream : ITrackStream
    {
        #region Fields

        private List<IEnumerable<Track>> _tracks;
        private readonly IEnumerator _enumerator;

        #endregion Fields

        #region Constructors

        public TrackStream(IEnumerable<Track> tracks)
        {
            _tracks = new List<IEnumerable<Track>>(new[] { tracks });
            _enumerator = _tracks.GetEnumerator();

            SupportsTrackSkipping = true;
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<Track> Current
        {
            get { return _enumerator.Current as IEnumerable<Track>; }
        }

        public bool SupportsTrackSkipping
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
        }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        #endregion Methods
    }

    public static class TrackStreamExtensions
    {
        public static ITrackStream ToTrackStream(this IEnumerable<Track> tracks)
        {
            return new TrackStream(tracks);
        }
    }
}