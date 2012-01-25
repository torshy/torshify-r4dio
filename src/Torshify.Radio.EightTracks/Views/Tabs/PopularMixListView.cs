using System.ComponentModel.Composition;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(PopularMixListView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PopularMixListView : MixListView
    {
        public PopularMixListView()
        {
            InitializeComponent();
        }

        [Import]
        public PopularMixListViewModel Model
        {
            get { return DataContext as PopularMixListViewModel; }
            set { DataContext = value; }
        }
    }
}