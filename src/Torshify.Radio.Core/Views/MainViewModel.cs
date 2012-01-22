using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.Core.Views
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : NotificationObject, INavigationAware
    {
        #region Constructors

        public MainViewModel()
        {
            NavigateBackCommand = new AutomaticCommand(ExecuteNavigateBack, CanExecuteNavigateBack);
            NavigateForwardCommand = new AutomaticCommand(ExecuteNavigateForward, CanExecuteNavigateForward);
        }

        #endregion Constructors

        #region Properties

        [Import]
        public IRegionManager RegionManager
        {
            get; 
            set;
        }

        public AutomaticCommand NavigateBackCommand
        {
            get; private set;
        }

        public AutomaticCommand NavigateForwardCommand
        {
            get; private set;
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
        }

        private bool CanExecuteNavigateBack()
        {
            return RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.CanGoBack;
        }

        private bool CanExecuteNavigateForward()
        {
            return RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.CanGoForward;
        }

        private void ExecuteNavigateBack()
        {
            RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.GoBack();
        }

        private void ExecuteNavigateForward()
        {
            RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.GoForward();
        }

        #endregion Methods
    }
}