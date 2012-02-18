using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Views.Settings.Tabs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NotGeneralSettingsViewModel : NotificationObject, IHeaderInfoProvider<HeaderInfo>
    {
        public NotGeneralSettingsViewModel()
        {
            HeaderInfo = new HeaderInfo
            {
                Title = "Other settings",
                IconUri = AppIcons.Settings.ToString()
            };
        }

        public HeaderInfo HeaderInfo
        {
            get; private set;
        }
    }
}