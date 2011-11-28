using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Torshify.Radio.EightTracks
{
    public partial class EightTracksRadioStationView : UserControl
    {
        #region Fields

        private DispatcherTimer _deferredCompleteTimer;

        #endregion Fields

        #region Constructors

        public EightTracksRadioStationView(EightTracksRadioStationViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;

            _deferredCompleteTimer = new DispatcherTimer();
            _deferredCompleteTimer.Interval = TimeSpan.FromMilliseconds(500);
            _deferredCompleteTimer.Tick += OnDeferredCompleteTimerTick;
        }

        #endregion Constructors

        #region Properties

        public EightTracksRadioStationViewModel Model
        {
            get { return DataContext as EightTracksRadioStationViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void AutoCompleteBox_TextChanged(object sender, RoutedEventArgs e)
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
    }
}