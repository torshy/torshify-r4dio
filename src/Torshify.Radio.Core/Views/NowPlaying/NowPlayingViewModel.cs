using System.ComponentModel.Composition;
using System.Windows.Input;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.Core.Views.NowPlaying
{
    [Export(typeof(NowPlayingViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public class NowPlayingViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private IRegionNavigationService _navigationService;

        #endregion Fields

        #region Constructors

        public NowPlayingViewModel()
        {
            NavigateBackCommand = new AutomaticCommand(ExecuteNavigateBack, CanExecuteNavigateBack);
        }

        #endregion Constructors

        #region Properties

        public ICommand NavigateBackCommand
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
            _navigationService = navigationContext.NavigationService;
        }

        private bool CanExecuteNavigateBack()
        {
            return _navigationService != null && _navigationService.Journal.CanGoBack;
        }

        private void ExecuteNavigateBack()
        {
            _navigationService.Journal.GoBack();
        }

        #endregion Methods
    }
}