using System;
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
        #region Fields

        private RectLatLng _currentViewArea;
        private GMapControl _map;

        #endregion Fields

        #region Constructors

        public TrackDJSetupView()
        {
            InitializeComponent();

            _currentViewArea = new RectLatLng();

            Loaded += SetupViewLoaded;
            Unloaded += SetupViewUnloaded;
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

        #region Methods

        private void MapOnOnMapZoomChanged()
        {
            _currentViewArea = _map.ViewArea;
        }

        private void MapOnOnPositionChanged(PointLatLng point)
        {
            _currentViewArea = _map.ViewArea;
        }

        private void SetupViewLoaded(object sender, RoutedEventArgs e)
        {
            _map = new GMapControl();
            _map.MapProvider = GMapProviders.OviMap;
            _map.MaxZoom = 24;
            _map.Zoom = 1;
            _map.OnPositionChanged += MapOnOnPositionChanged;
            _map.OnMapZoomChanged += MapOnOnMapZoomChanged;

            if (!_currentViewArea.IsEmpty)
            {
                _map.SetZoomToFitRect(_currentViewArea);
            }

            Model.GetViewArea = () => _currentViewArea;
            Model.GetSelectedArea = () =>
                                        {
                                            if (_map != null)
                                            {
                                                return _map.SelectedArea;
                                            }

                                            return new RectLatLng();
                                        };

            MapDecorator.Child = _map;
        }

        private void SetupViewUnloaded(object sender, RoutedEventArgs e)
        {
            _map.OnMapZoomChanged -= MapOnOnMapZoomChanged;
            _map.OnPositionChanged -= MapOnOnPositionChanged;
            _map = null;

            MapDecorator.Child = null;
        }

        #endregion Methods
    }
}