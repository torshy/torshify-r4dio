using System.ComponentModel.Composition;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(RecentMixListView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RecentMixListView : MixListView
    {
        public RecentMixListView()
        {
            InitializeComponent();
        }

        [Import]
        public RecentMixListViewModel Model
        {
            get { return DataContext as RecentMixListViewModel; }
            set { DataContext = value; }
        }  
    }
}