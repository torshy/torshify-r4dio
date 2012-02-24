using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.EchoNest.Views.Style
{
    [Export]
    [RegionMemberLifetime(KeepAlive = false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class MainStationView : UserControl
    {
        public MainStationView()
        {
            InitializeComponent();
        }

        [Import]
        public MainStationViewModel Model
        {
            get
            {
                return DataContext as MainStationViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
