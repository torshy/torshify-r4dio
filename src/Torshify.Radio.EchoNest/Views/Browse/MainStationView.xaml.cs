using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.EchoNest.Views.Browse
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class MainStationView : UserControl
    {
        #region Fields

        public const string TabViewRegion = "BrowseArtistsRegion";

        #endregion Fields

        #region Constructors

        public MainStationView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public MainStationViewModel Model
        {
            get { return DataContext as MainStationViewModel; }
            set
            {
                DataContext = value;

                RegionManager.SetRegionManager(_tabControl, value.RegionManager);
                RegionManager.SetRegionName(_tabControl, TabViewRegion);
            }
        }

        #endregion Properties
    }
}