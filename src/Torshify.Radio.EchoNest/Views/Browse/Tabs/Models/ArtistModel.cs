using System.Collections.Generic;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Browse.Tabs.Models
{
    public class ArtistModel : NotificationObject
    {
        #region Fields

        private IEnumerable<TrackContainer> _albums;
        private string _name;

        #endregion Fields

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
                    _albums = value;
                    RaisePropertyChanged("Albums");
                }
            }
        }

        #endregion Properties
    }
}