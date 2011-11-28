using System;
using System.ComponentModel;

namespace Torshify.Radio.Framework
{
    public class RadioTrack : IRadioTrack, INotifyPropertyChanged
    {
        #region Fields

        private string _album;
        private string _albumArt;
        private string _artist;
        private string _name;
        private TimeSpan _totalDuration;

        #endregion Fields

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public string Album
        {
            get { return _album; }
            set
            {
                _album = value;
                OnPropertyChanged("Album");
            }
        }

        public string AlbumArt
        {
            get { return _albumArt; }
            set
            {
                _albumArt = value;
                OnPropertyChanged("AlbumArt");
            }
        }

        public TimeSpan TotalDuration
        {
            get { return _totalDuration; }
            set
            {
                _totalDuration = value;
                OnPropertyChanged("TotalDuration");
            }
        }

        public string Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                OnPropertyChanged("Artist");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        #endregion Properties

        #region Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Methods
    }
}