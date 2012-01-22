using System.ComponentModel.Composition;
using Microsoft.Practices.Prism;
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
            SearchCommand = new AutomaticCommand<string>(ExecuteSearch, CanExecuteSearch);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand NavigateBackCommand
        {
            get; private set;
        }

        public AutomaticCommand NavigateForwardCommand
        {
            get; private set;
        }

        [Import]
        public IRegionManager RegionManager
        {
            get;
            set;
        }

        [Import]
        public ISearchBarService SearchBarService
        {
            get;
            set;
        }

        public AutomaticCommand<string> SearchCommand
        {
            get;
            private set;
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

        private bool CanExecuteSearch(string phrase)
        {
            return !string.IsNullOrEmpty(phrase);
        }

        private void ExecuteNavigateBack()
        {
            RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.GoBack();
        }

        private void ExecuteNavigateForward()
        {
            RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.GoForward();
        }

        private void ExecuteSearch(string phrase)
        {
            RegionManager.RequestNavigate(AppRegions.ViewRegion, SearchBarService.Current.NavigationUri);
        }

        #endregion Methods
    }
}