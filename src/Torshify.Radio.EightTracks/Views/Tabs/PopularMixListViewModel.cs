using System.ComponentModel.Composition;
using System.Linq;
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
                                                     SearchForMixes(global::EightTracks.Mixes.Sort.Popular);
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