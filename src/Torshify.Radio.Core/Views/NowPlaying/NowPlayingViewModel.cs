using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.Core.Views.NowPlaying
{
    [Export(typeof(NowPlayingViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public class NowPlayingViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IRadio _radio;

        private IRegionNavigationService _navigationService;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public NowPlayingViewModel(IRadio radio)
        {
            _radio = radio;

            NavigateBackCommand = new AutomaticCommand(ExecuteNavigateBack, CanExecuteNavigateBack);
        }

        #endregion Constructors

        #region Properties

        public ICommand NavigateBackCommand
        {
            get; private set;
        }

        public IRadio Radio
        {
            get { return _radio; }
        }

        #endregion Properties

        #region Methods

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            _radio.CurrentTrackChanged -= RadioOnCurrentTrackChanged;
            _radio.UpcomingTrackChanged -= RadioOnUpcomingTrackChanged;
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            _radio.CurrentTrackChanged += RadioOnCurrentTrackChanged;
            _radio.UpcomingTrackChanged += RadioOnUpcomingTrackChanged;
        }

        private void RadioOnUpcomingTrackChanged(object sender, EventArgs eventArgs)
        {
        }

        private void RadioOnCurrentTrackChanged(object sender, EventArgs eventArgs)
        {
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