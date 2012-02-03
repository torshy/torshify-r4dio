using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.EchoNest.Views.Similar.Tabs;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Similar
{
    [Export(typeof(MainStationViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public class MainStationViewModel : NotificationObject, IRadioStation
    {
        #region Constructors

        public MainStationViewModel()
        {
            RegionManager = new RegionManager();
        }

        #endregion Constructors

        #region Properties

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

        public void OnTuneIn(NavigationContext context)
        {
            SearchBarService.SetActive(bar => bar.NavigationUri.OriginalString.StartsWith(context.Uri.OriginalString));

            RegionManager.RequestNavigate(
                MainStationView.TabViewRegion,
                typeof(SimilarView).FullName + context.Parameters);
        }

        public void OnTuneAway(NavigationContext context)
        {
        }

        #endregion Methods
    }
}