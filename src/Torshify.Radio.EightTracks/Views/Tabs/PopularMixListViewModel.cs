using System.ComponentModel.Composition;
using System.Linq;
using EightTracks;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(PopularMixListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PopularMixListViewModel : MixListViewModel
    {
        private HeaderInfo _header;

        public PopularMixListViewModel()
        {
            _header = new HeaderInfo
                      {
                          Title = "Popular",
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
            get { return global::EightTracks.Mixes.Sort.Popular; }
        }
    }
}