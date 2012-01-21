namespace Torshify.Radio.Framework
{
    public class TrackBufferingEventArgs : TrackEventArgs
    {
        #region Constructors

        public TrackBufferingEventArgs(Track track, double progress)
            : base(track)
        {
            Progress = progress;
        }

        #endregion Constructors

        #region Properties

        public double Progress
        {
            get; private set;
        }

        #endregion Properties
    }
}