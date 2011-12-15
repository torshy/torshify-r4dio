using System.ComponentModel.Composition;
using System.Windows.Controls;

using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.EchoNest.TrackDJ
{
    [Export(typeof(TrackDJView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TrackDJView : UserControl
    {
        #region Fields

        private IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        public TrackDJView()
        {
            InitializeComponent();

            _regionManager = new RegionManager();
            _regionManager.RegisterViewWithRegion("TrackDJMainRegion", typeof(TrackDJSetupView));
            _regionManager.RegisterViewWithRegion("TrackDJMainRegion", typeof(TrackDJResultsView));

            RegionManager.SetRegionManager(_regionHost, _regionManager);
            RegionManager.SetRegionName(_regionHost, "TrackDJMainRegion");

            _regionManager.RequestNavigate("TrackDJMainRegion", typeof(TrackDJSetupView).FullName);
        }

        #endregion Constructors

        #region Properties

        [Import]
        public TrackDJViewModel Model
        {
            get
            {
                return DataContext as TrackDJViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion Properties
    }
}