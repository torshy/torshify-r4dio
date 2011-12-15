using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace Torshify.Radio.EchoNest.TrackDJ
{
    [Export(typeof(TrackDJSetupView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TrackDJSetupView : UserControl
    {
        private GMapControl _map;

        #region Constructors

        public TrackDJSetupView()
        {
            InitializeComponent();

            Loaded += SetupViewLoaded;
            Unloaded += SetupViewUnloaded;
            
        }

        private void SetupViewUnloaded(object sender, RoutedEventArgs e)
        {
            _map = null;
            MapDecorator.Child = null;
        }

        private void SetupViewLoaded(object sender, RoutedEventArgs e)
        {
            _map = new GMapControl();
            _map.MapProvider = GMapProviders.OviMap;
            _map.MaxZoom = 24;
            _map.Zoom = 1;

            Model.GetViewArea = () => _map.ViewArea;
            Model.GetSelectedArea = () => _map.SelectedArea;

            MapDecorator.Child = _map;
        }

        #endregion Constructors

        #region Properties

        [Import]
        public TrackDJSetupViewModel Model
        {
            get
            {
                return DataContext as TrackDJSetupViewModel;
            }
            set
            {
                DataContext = value;

            }
        }

        #endregion Properties
    }
}