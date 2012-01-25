using System.ComponentModel.Composition;
using System.Linq;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(RecentMixListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RecentMixListViewModel : MixListViewModel
    {
        private HeaderInfo _header;

        public RecentMixListViewModel()
        {
            _header = new HeaderInfo
                      {
                          Title = "Recent",
                          IsSelectedAction = isSelected =>
                                             {
                                                 if (!Mixes.Any() && isSelected)
                                                 {
                                                     SearchForMixes(global::EightTracks.Mixes.Sort.Recent);
                                                 }
                                             }
                      };
        }

        public override HeaderInfo HeaderInfo
        {
            get { return _header; }
        }
    }
}