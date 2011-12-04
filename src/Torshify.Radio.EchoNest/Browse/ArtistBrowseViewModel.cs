using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest.Browse
{
    [Export(typeof(ArtistBrowseViewModel))]
    public class ArtistBrowseViewModel : NotificationObject, INavigationAware
    {
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}