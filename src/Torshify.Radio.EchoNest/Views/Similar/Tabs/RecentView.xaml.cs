using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.Views.Similar.Tabs
{
    [Export(typeof(RecentView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RecentView : UserControl
    {
        public RecentView()
        {
            InitializeComponent();
        }

        
        [Import]
        public RecentViewModel Model
        {
            get { return DataContext as RecentViewModel; }
            set { DataContext = value; }
        }
    }
}
