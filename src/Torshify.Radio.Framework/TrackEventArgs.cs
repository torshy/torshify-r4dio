using System;

namespace Torshify.Radio.Framework
{
    public class TrackEventArgs : EventArgs
    {
        #region Constructors

        public TrackEventArgs(Track track)
        {
            Track = track;
        }

        #endregion Constructors

        #region Properties

        public Track Track
        {
            get; private set;
        }

        #endregion Properties
    }
}