using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(MainTabViewModel))]
    public class MainTabViewModel : NotificationObject, IHeaderInfoProvider<HeaderInfo>
    {
        public MainTabViewModel()
        {
            HeaderInfo = new HeaderInfo
                         {
                             Title = "8tracks"
                         };
        }

        public HeaderInfo HeaderInfo
        {
            get; 
            private set;
        }
    }
}