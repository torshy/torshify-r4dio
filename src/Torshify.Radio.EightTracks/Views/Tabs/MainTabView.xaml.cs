using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(MainTabView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class MainTabView : UserControl
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
