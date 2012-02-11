using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;

namespace Torshify.Radio.Framework
{
    [DebuggerDisplay("{Name}")]
    public abstract class Track : INotifyPropertyChanged
    {
        #region Fields

        private string _album;
        private string _albumArt;
        private string _artist;
        private string _name;
        private TimeSpan _totalDuration;
        private int _index;

        #endregion Fields

        #region Constructors

        protected Track()
        {
            ExtraData = new ExpandoObject();
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public int Index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    OnPropertyChanged("Index");
                }
            }
        }

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

        public string Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                OnPropertyChanged("Artist");
            }
        }

        public dynamic ExtraData
        {
            get;
            private set;
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

        public TimeSpan TotalDuration
        {
            get { return _totalDuration; }
            set
            {
                _totalDuration = value;
                OnPropertyChanged("TotalDuration");
            }
        }

        #endregion Properties

        #region Methods

        public abstract TrackLink ToLink();

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