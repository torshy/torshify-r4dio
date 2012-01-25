using System.ComponentModel.Composition;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(HotMixListView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HotMixListView : MixListView
    {
        public HotMixListView()
        {
            InitializeComponent();
        }

        [Import]
        public HotMixListViewModel Model
        {
            get { return DataContext as HotMixListViewModel; }
            set { DataContext = value; }
        }
    }
}