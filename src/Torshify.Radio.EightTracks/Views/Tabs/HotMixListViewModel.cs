using System.ComponentModel.Composition;
using System.Linq;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(HotMixListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HotMixListViewModel : MixListViewModel
    {
        private HeaderInfo _header;

        public HotMixListViewModel()
        {
            _header = new HeaderInfo
                      {
                          Title = "Hot",
                          IsSelectedAction = isSelected =>
                                             {
                                                 if (!Mixes.Any() && isSelected)
                                                 {
                                                     SearchForMixes(global::EightTracks.Mixes.Sort.Hot);
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