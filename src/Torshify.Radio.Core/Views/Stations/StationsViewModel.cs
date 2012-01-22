using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
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

        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            if (_navigationService != null)
            {
                _navigationService.Region.NavigationService.Navigated -= NavigationServiceOnNavigated;
                _navigationService.Region.NavigationService.Navigating -= NavigationServiceOnNavigating;
            }

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
            var active = e.NavigationContext.NavigationService.Region.ActiveViews.FirstOrDefault() as FrameworkElement;

            if (active != null)
            {
                IRadioStation radioStation = active as IRadioStation;

                if (radioStation == null)
                {
                    radioStation = active.DataContext as IRadioStation;
                }

                if (radioStation != null)
                {
                    radioStation.OnTuneIn(e.NavigationContext);
                }
            }
        }

        private void NavigationServiceOnNavigating(object sender, RegionNavigationEventArgs e)
        {
            var active = e.NavigationContext.NavigationService.Region.ActiveViews.FirstOrDefault() as FrameworkElement;

            if (active != null)
            {
                IRadioStation radioStation = active as IRadioStation;

                if (radioStation == null)
                {
                    radioStation = active.DataContext as IRadioStation;
                }

                if (radioStation != null)
                {
                    radioStation.OnTuneAway(e.NavigationContext);
                }
            }
        }

        #endregion Methods
    }
}