using System;

namespace Torshify.Radio.Framework
{
    public class MediaPlayerRadioTrack : RadioTrack
    {
        #region Fields

        private Uri _uri;

        #endregion Fields

        #region Properties

        public Uri Uri
        {
            get { return _uri; }
            set
            {
                _uri = value;
                OnPropertyChanged("Uri");
            }
        }

        #endregion Properties
    }
}