using System;
using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public class TrackProvider
    {
        #region Constructors

        public TrackProvider()
        {
        }

        public TrackProvider(Func<IEnumerable<RadioTrack>> batchProvider, bool canSkipTracks = true)
        {
            CanSkipTracks = canSkipTracks;
            BatchProvider = batchProvider;
        }

        #endregion Constructors

        #region Properties

        public Func<IEnumerable<RadioTrack>> BatchProvider
        {
            get;
            set;
        }

        public bool CanSkipTracks
        {
            get;
            set;
        }

        #endregion Properties
    }
}