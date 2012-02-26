using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Torshify.Radio.Spotify.Views.Settings
{
    public partial class SpotifyLoginSectionView : UserControl
    {
        #region Fields

        private DispatcherTimer _deferredLoginTimer;

        #endregion Fields

        #region Constructors

        public SpotifyLoginSectionView()
        {
            InitializeComponent();

            _deferredLoginTimer = new DispatcherTimer();
            _deferredLoginTimer.Interval = TimeSpan.FromMilliseconds(750);
            _deferredLoginTimer.Tick += DeferredLoginTimerOnTick;
        }

        #endregion Constructors

        #region Properties

        public SpotifyLoginSection Model
        {
            get
            {
                return DataContext as SpotifyLoginSection;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion Properties

        #region Methods

        private void PasswordBoxPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            _deferredLoginTimer.Stop();
            _deferredLoginTimer.Start();
        }

        private void DeferredLoginTimerOnTick(object sender, EventArgs eventArgs)
        {
            _deferredLoginTimer.Stop();

            Model.Login(_userName.Text, _password.Password);
        }

        #endregion Methods
    }
}