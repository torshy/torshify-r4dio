using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Regions;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Browse
{
    [Export(typeof(BrowseView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
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
            _regionManager.RegisterViewWithRegion("BrowseMainRegion", typeof(BrowseViewStartPage));
            _regionManager.RegisterViewWithRegion("BrowseMainRegion", typeof(SearchResultsView));
            _regionManager.RegisterViewWithRegion("BrowseMainRegion", typeof(ArtistBrowseView));

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

        private void UserControlPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var region = _regionManager.Regions["BrowseMainRegion"];

            if (e.XButton1 == MouseButtonState.Pressed)
            {
                if (region.NavigationService.Journal.CanGoBack)
                {
                    region.NavigationService.Journal.GoBack();
                    e.Handled = true;
                }
            }
            else if (e.XButton2 == MouseButtonState.Pressed)
            {
                if (region.NavigationService.Journal.CanGoForward)
                {
                    region.NavigationService.Journal.GoForward();
                    e.Handled = true;
                }
            }
        }

        #endregion Methods
    }
}