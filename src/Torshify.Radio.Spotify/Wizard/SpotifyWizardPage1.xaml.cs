using System;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Windows.Controls;

using Torshify.Radio.Framework;
using Torshify.Radio.Spotify.LoginService;

namespace Torshify.Radio.Spotify.Wizard
{
    [Export(typeof(WizardPage))]
    public partial class SpotifyWizardPage1 : WizardPage, LoginServiceCallback
    {
        #region Fields

        private LoginServiceClient _client;
        private DispatcherTimer _deferredLoginTimer;

        #endregion Fields

        #region Constructors

        public SpotifyWizardPage1()
        {
            InitializeComponent();
            _deferredLoginTimer = new DispatcherTimer();
            _deferredLoginTimer.Interval = TimeSpan.FromMilliseconds(750);
            _deferredLoginTimer.Tick += DeferredLoginTimerOnTick;
            _client = new LoginServiceClient(new InstanceContext(this));

            LoadingIndicator = new LoadingIndicatorService();

            DataContext = this;
        }

        #endregion Constructors

        #region Properties

        public ILoadingIndicatorService LoadingIndicator
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        void LoginServiceCallback.OnLoggedIn()
        {
            LoadingIndicator.Pop();
            Dispatcher.BeginInvoke(new Action<bool>(result =>
                                                    {
                                                        CanSelectNextPage = result;
                                                        CommandManager.InvalidateRequerySuggested();
                                                    }), true);

            Dispatcher.BeginInvoke(new Action<string>(txt => _statusText.Text = txt), "Successfully logged in");
        }

        void LoginServiceCallback.OnLoginError(string message)
        {
            LoadingIndicator.Pop();
            Dispatcher.BeginInvoke(new Action<string>(txt => _statusText.Text = txt), message);
        }

        void LoginServiceCallback.OnLoggedOut()
        {
            Dispatcher.BeginInvoke(new Action<bool>(result => CanSelectNextPage = result), false);
        }

        void LoginServiceCallback.OnPing()
        {
        }

        private void DeferredLoginTimerOnTick(object sender, EventArgs eventArgs)
        {
            _deferredLoginTimer.Stop();

            try
            {
                LoadingIndicator.Push();

                if (_client.State != CommunicationState.Opened)
                {
                    _client = new LoginServiceClient(new InstanceContext(this));
                }

                _client.Subscribe();
                _client.Login(_userName.Text, _password.Password, true);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                _client.Abort();
            }
        }

        private void PasswordBoxPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            _deferredLoginTimer.Stop();
            _deferredLoginTimer.Start();
        }

        private void UsernameTextBoxChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(_userName.Text))
            {
                CanSelectNextPage = true;
            }
            else
            {
                CanSelectNextPage = !string.IsNullOrEmpty(_password.Password);
            }
        }

        private void WizardPageUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (_client != null)
                {
                    _client.Close();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion Methods
    }
}