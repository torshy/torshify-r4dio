using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.EchoNest.Browse
{
    [Export(typeof(BrowseView))]
    public partial class BrowseView : UserControl
    {
        #region Fields

        private readonly DispatcherTimer _deferredCompleteTimer;
        private IRegionManager _regionManager;
        #endregion Fields

        #region Constructors

        public BrowseView()
        {
            InitializeComponent();

            _regionManager = new RegionManager();
            RegionManager.SetRegionManager(_regionHost, _regionManager);
            RegionManager.SetRegionName(_regionHost, "BrowseMainRegion");

            _deferredCompleteTimer = new DispatcherTimer();
            _deferredCompleteTimer.Interval = TimeSpan.FromMilliseconds(100);
            _deferredCompleteTimer.Tick += OnDeferredCompleteTimerTick;
        }

        #endregion Constructors

        #region Properties

        [Import]
        public BrowseViewModel Model
        {
            get
            {
                return DataContext as BrowseViewModel;
            }
            set
            {
                DataContext = value;

                value.RegionManager = _regionManager;
            }
        }

        #endregion Properties

        #region Methods

        private void AutoCompleteBoxTextChanged(object sender, RoutedEventArgs e)
        {
            _deferredCompleteTimer.Stop();
            _deferredCompleteTimer.Start();
        }

        private void OnDeferredCompleteTimerTick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_autocomplete.SearchText))
            {
                Model.UpdateAutoComplete(_autocomplete.SearchText);
            }

            _deferredCompleteTimer.Stop();
        }

        #endregion Methods

        private void UserControl_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.XButton1 == MouseButtonState.Pressed)
            _regionManager.Regions["BrowseMainRegion"].NavigationService.Journal.GoBack();
        }
    }
}