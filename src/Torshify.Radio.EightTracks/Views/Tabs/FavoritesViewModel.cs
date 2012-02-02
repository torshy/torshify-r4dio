using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(FavoritesViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FavoritesViewModel : NotificationObject, IHeaderInfoProvider<HeaderInfo>
    {
        public FavoritesViewModel()
        {
            HeaderInfo = new HeaderInfo
                         {
                             Title = "Favorites"
                         };
        }

        public HeaderInfo HeaderInfo
        {
            get; 
            private set;
        }
    }
}