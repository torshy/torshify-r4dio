using System;

namespace Torshify.Radio.Framework
{
    public class TrackEventArgs : EventArgs
    {
        #region Constructors

        public TrackEventArgs(IRadioTrackPlayer source, IRadioTrack track)
        {
            Source = source;
            Track = track;
        }

        #endregion Constructors

        #region Properties

        public IRadioTrackPlayer Source
        {
            get; private set;
        }

        public IRadioTrack Track
        {
            get; private set;
        }

        #endregion Properties
    }
}