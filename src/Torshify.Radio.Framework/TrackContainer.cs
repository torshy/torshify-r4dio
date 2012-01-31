using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;

namespace Torshify.Radio.Framework
{
    [DebuggerDisplay("{Name}")]
    public class TrackContainer : INotifyPropertyChanged
    {
        #region Fields

        private string _image;
        private string _name;
        private TrackContainerOwner _owner;
        private IEnumerable<Track> _tracks;
        private int _year;

        #endregion Fields

        #region Constructors

        public TrackContainer()
        {
            Tracks = new ObservableCollection<Track>();
            ExtraData = new ExpandoObject();
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public dynamic ExtraData
        {
            get;
            private set;
        }

        public string Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    RaisePropertyChanged("Image");
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public int Year
        {
            get { return _year; }
            set
            {
                if (_year != value)
                {
                    _year = value;
                    RaisePropertyChanged("Year");
                }
            }
        }

        public TrackContainerOwner Owner
        {
            get { return _owner; }
            set
            {
                if (_owner != value)
                {
                    _owner = value;
                    RaisePropertyChanged("Owner");
                }
            }
        }

        public IEnumerable<Track> Tracks
        {
            get { return _tracks; }
            set
            {
                if (_tracks != value)
                {
                    _tracks = value;
                    RaisePropertyChanged("Tracks");
                }
            }
        }

        #endregion Properties

        #region Methods

        protected virtual void RaisePropertyChanged(string propertyName)
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