using System;

namespace Torshify.Radio.Framework
{
    public class TrackChangedEventArgs : EventArgs
    {
        #region Constructors

        public TrackChangedEventArgs(Track previousTrack, Track currentTrack)
        {
            PreviousTrack = previousTrack;
            CurrentTrack = currentTrack;
        }

        #endregion Constructors

        #region Properties

        public Track PreviousTrack
        {
            get; private set;
        }

        public Track CurrentTrack
        {
            get; private set;
        }

        #endregion Properties
    }
}