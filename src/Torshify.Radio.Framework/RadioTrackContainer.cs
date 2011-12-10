using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;

namespace Torshify.Radio.Framework
{
    public class RadioTrackContainer : INotifyPropertyChanged
    {
        #region Fields

        private string _artistName;
        private string _containerArt;
        private string _name;
        private IEnumerable<RadioTrack> _tracks;

        #endregion Fields

        #region Constructors

        public RadioTrackContainer()
        {
            Tracks = new ObservableCollection<RadioTrack>();
            ExtraData = new ExpandoObject();
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public string ArtistName
        {
            get { return _artistName; }
            set
            {
                if (_artistName != value)
                {
                    _artistName = value;
                    RaisePropertyChanged("ArtistName");
                }
            }
        }

        public string ContainerArt
        {
            get { return _containerArt; }
            set
            {
                if (_containerArt != value)
                {
                    _containerArt = value;
                    RaisePropertyChanged("ContainerArt");
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

        public IEnumerable<RadioTrack> Tracks
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

        public dynamic ExtraData
        {
            get; 
            private set;
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