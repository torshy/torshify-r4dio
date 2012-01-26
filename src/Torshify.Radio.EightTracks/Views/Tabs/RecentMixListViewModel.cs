using System.ComponentModel.Composition;
using System.Linq;
using EightTracks;
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
                                                     SearchForMixes(SortType);
                                                 }
                                             }
                      };
        }

        public override HeaderInfo HeaderInfo
        {
            get { return _header; }
        }

        protected override Mixes.Sort SortType
        {
            get { return global::EightTracks.Mixes.Sort.Recent; }
        }
    }
}