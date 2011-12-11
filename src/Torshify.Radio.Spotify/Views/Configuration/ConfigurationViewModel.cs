using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;
using Torshify.Radio.Spotify.LoginService;

namespace Torshify.Radio.Spotify.Views.Configuration
{
    [ConfigurationMetadata(Name = "Spotify", Icon = "pack://siteoforigin:,,,/Modules/Spotify/Resources/Icons/Spotify_Logo.png")]
    public class ConfigurationViewModel : NotificationObject, IConfiguration, LoginServiceCallback
    {
        #region Fields

        private bool _rememberMe;
        private string _userName;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public ConfigurationViewModel(ConfigurationView view)
        {
            LoginCommand = new AutomaticCommand<PasswordBox>(ExecuteLogin, CanExecuteLogin);
            UI = view;
            UI.DataContext = this;
        }

        #endregion Constructors

        #region Properties

        public ICommand LoginCommand
        {
            get;
            private set;
        }

        public bool RememberMe
        {
            get { return _rememberMe; }
            set
            {
                if (_rememberMe != value)
                {
                    _rememberMe = value;
                    RaisePropertyChanged("RememberMe");
                }
            }
        }

        public FrameworkElement UI
        {
            get;
            private set;
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }

        #endregion Properties

        #region Methods

        public void Cancel()
        {
        }

        public void Commit()
        {
        }

        public void Initialize(IConfigurationContext context)
        {
            LoginServiceClient login = new LoginServiceClient(new InstanceContext(this));

            try
            {
                UserName = login.GetRememberedUser();
                RememberMe = !string.IsNullOrEmpty(UserName);
                login.Close();
            }
            catch
            {
                login.Abort();
            }
        }

        void LoginServiceCallback.OnLoggedIn()
        {
        }

        void LoginServiceCallback.OnLoggedOut()
        {
        }

        void LoginServiceCallback.OnLoginError(string message)
        {
        }

        void LoginServiceCallback.OnPing()
        {
        }

        private bool CanExecuteLogin(PasswordBox box)
        {
            return !string.IsNullOrEmpty(UserName) && box.SecurePassword.Length > 0;
        }

        private void ExecuteLogin(PasswordBox box)
        {
            LoginServiceClient login = new LoginServiceClient(new InstanceContext(this));

            try
            {
                if (!string.IsNullOrEmpty(login.GetRememberedUser()) && !RememberMe)
                {
                    login.ForgetRememberedUser();
                }

                login.Login(UserName, box.Password, RememberMe);
                login.Close();
            }
            catch
            {
                login.Abort();
            }
        }

        #endregion Methods
    }
}