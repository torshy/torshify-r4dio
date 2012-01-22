using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(TagsTabViewModel))]
    public class TagsTabViewModel : NotificationObject, IHeaderInfoProvider<HeaderInfo>
    {
        #region Constructors

        public TagsTabViewModel()
        {
            HeaderInfo = new HeaderInfo
                         {
                             Title = "Tags"
                         };
        }

        #endregion Constructors

        #region Properties

        public HeaderInfo HeaderInfo
        {
            get;
            private set;
        }

        #endregion Properties
    }
}