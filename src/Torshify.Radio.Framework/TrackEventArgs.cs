using System;

namespace Torshify.Radio.Framework
{
    public class TrackEventArgs : EventArgs
    {
        #region Constructors

        public TrackEventArgs(IRadioTrackPlayer source, RadioTrack track)
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

        public RadioTrack Track
        {
            get; private set;
        }

        #endregion Properties
    }
}