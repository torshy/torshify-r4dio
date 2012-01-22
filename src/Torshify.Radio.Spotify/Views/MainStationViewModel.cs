using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Spotify.Views
{
    [Export(typeof(MainStationViewModel))]
    public class MainStationViewModel : NotificationObject, INavigationAware
    {
        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}