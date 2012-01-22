using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.EightTracks.Views.Tabs;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.Views
{
    [Export(typeof(MainStationViewModel))]
    public class MainStationViewModel : NotificationObject, IRadioStation
    {
        #region Constructors

        public MainStationViewModel()
        {
            RegionManager = new RegionManager();
            RegionManager.RegisterViewWithRegion(MainStationView.TabViewRegion, typeof(MainTabView));
            RegionManager.RegisterViewWithRegion(MainStationView.TabViewRegion, typeof(TagsTabView));
        }

        #endregion Constructors

        #region Properties

        public IRegionManager RegionManager
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public void OnTuneAway(NavigationContext context)
        {
        }

        public void OnTuneIn(NavigationContext context)
        {
        }

        #endregion Methods
    }
}