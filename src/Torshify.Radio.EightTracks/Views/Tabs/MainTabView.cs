using System.ComponentModel.Composition;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(MainTabView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainTabView : MixListView
    {
        public MainTabView()
        {
            InitializeComponent();
        }

        [Import]
        public MainTabViewModel Model
        {
            get { return DataContext as MainTabViewModel; }
            set { DataContext = value; }
        }
    }
}