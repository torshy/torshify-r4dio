using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.Core.Views.Stations
{
    [Export(typeof(StationsViewModel))]
    public class StationsViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private IRegionNavigationService _navigationService;

        #endregion Fields

        #region Constructors

        public StationsViewModel()
        {
            NavigateToTileCommand = new AutomaticCommand<Tile>(ExecuteNavigateToTile, CanExecuteNavigateToTile);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand<Tile> NavigateToTileCommand
        {
            get;
            private set;
        }

        [Import]
        public ITileService TileService
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (_navigationService != null)
            {
                _navigationService.Region.NavigationService.Navigated -= NavigationServiceOnNavigated;
                _navigationService.Region.NavigationService.Navigating -= NavigationServiceOnNavigating;
                _navigationService = null;
            }
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
            _navigationService.Region.NavigationService.Navigated += NavigationServiceOnNavigated;
            _navigationService.Region.NavigationService.Navigating += NavigationServiceOnNavigating;
        }

        private bool CanExecuteNavigateToTile(Tile tile)
        {
            return tile != null;
        }

        private void ExecuteNavigateToTile(Tile tile)
        {
            _navigationService.RequestNavigate(tile.NavigationUri);
        }

        private void NavigationServiceOnNavigated(object sender, RegionNavigationEventArgs e)
        {
            var activeStation = e.NavigationContext.NavigationService.Region.ActiveViews.FirstOrDefault() as IRadioStation;

            if (activeStation != null)
            {
                activeStation.OnTuneIn();
            }
        }

        private void NavigationServiceOnNavigating(object sender, RegionNavigationEventArgs e)
        {
            var activeStation = e.NavigationContext.NavigationService.Region.ActiveViews.FirstOrDefault() as IRadioStation;

            if (activeStation != null)
            {
                activeStation.OnTuneAway();
            }
        }

        #endregion Methods
    }
}