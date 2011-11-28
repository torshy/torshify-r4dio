using System;

namespace Torshify.Radio.Framework
{
    public interface IRadioTrack
    {
        #region Properties

        string Name
        {
            get;
        }

        string Album
        {
            get;
        }

        string Artist
        {
            get;
        }

        string AlbumArt
        {
            get;
        }

        TimeSpan TotalDuration
        {
            get;
        }

        #endregion Properties
    }
}