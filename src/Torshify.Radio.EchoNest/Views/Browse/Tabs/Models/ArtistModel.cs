using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Browse.Tabs.Models
{
    public class ArtistModel : NotificationObject
    {
        #region Fields

        private ObservableCollection<TrackContainer> _albums;
        private string _name;

        #endregion Fields

        public ArtistModel()
        {
            ArtistInfo = new ArtistInformationContainer();
        }

        #region Properties

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

        public IEnumerable<TrackContainer> Albums
        {
            get { return _albums; }
            set
            {
                if (_albums != value)
                {
                    _albums = new ObservableCollection<TrackContainer>(value);
                    _albums.Insert(0, ArtistInfo);
                    RaisePropertyChanged("Albums");
                }
            }
        }

        public ArtistInformationContainer ArtistInfo
        {
            get; 
            private set;
        }

        #endregion Properties
    }
}