using System;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.EightTracks.Views.Tabs;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.Views
{
    [Export(typeof(MainStationViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainStationViewModel : NotificationObject, IRadioStation, IRegionMemberLifetime
    {
        #region Constructors

        public MainStationViewModel()
        {
            RegionManager = new RegionManager();
        }

        #endregion Constructors

        #region Properties

        public bool KeepAlive
        {
            get { return false; }
        }

        public IRegionManager RegionManager
        {
            get;
            private set;
        }

        [Import]
        public ISearchBarService SearchBarService
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void OnTuneAway(NavigationContext context)
        {
        }

        public void OnTuneIn(NavigationContext context)
        {
            // TODO : Add a helper method to do this much easier
            if (!SearchBarService.Current.NavigationUri.OriginalString.StartsWith(typeof(MainStationView).FullName))
            {
                SearchBarService.Current = SearchBarService.SearchBars.FirstOrDefault(
                    s => s.NavigationUri.OriginalString.StartsWith(typeof(MainStationView).FullName));
            }

            if (!string.IsNullOrEmpty(context.Parameters[SearchBar.IsFromSearchBarParameter]))
            {
                RegionManager.RequestNavigate(
                    MainStationView.TabViewRegion,
                    typeof(MainTabView).FullName + context.Parameters);
            }
            else
            {
                RegionManager.RequestNavigate(
                    MainStationView.TabViewRegion,
                    typeof (MainTabView).FullName);
            }
        }

        #endregion Methods
    }
}