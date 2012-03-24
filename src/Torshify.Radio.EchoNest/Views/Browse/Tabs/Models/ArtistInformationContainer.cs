using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Browse.Tabs.Models
{
    public class ArtistInformationContainer : TrackContainer
    {
        #region Fields

        private string _biography;
        private string _yearsActive;

        #endregion Fields

        #region Properties

        public string Biography
        {
            get { return _biography; }
            set
            {
                _biography = value;
                RaisePropertyChanged("Biography");
            }
        }

        public string YearsActive
        {
            get { return _yearsActive; }
            set
            {
                _yearsActive = value;
                RaisePropertyChanged("YearsActive");
            }
        }

        #endregion Properties
    }
}