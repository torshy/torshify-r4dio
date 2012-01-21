using System;

namespace Torshify.Radio.Framework
{
    public class TrackProgressEventArgs : EventArgs
    {
        #region Constructors

        public TrackProgressEventArgs(double totalMilliseonds, double elapsedMilliseconds)
        {
            TotalMilliseonds = totalMilliseonds;
            ElapsedMilliseconds = elapsedMilliseconds;
        }

        #endregion Constructors

        #region Properties

        public double ElapsedMilliseconds
        {
            get; private set;
        }

        public double TotalMilliseonds
        {
            get; private set;
        }
        
        #endregion Properties
    }
}