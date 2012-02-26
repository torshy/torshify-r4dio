using System;
using System.ServiceModel;
using System.Threading.Tasks;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;
using Torshify.Radio.Spotify.LoginService;

namespace Torshify.Radio.Spotify.Views.Settings
{
    public class SpotifyLoginSection : NotificationObject, ISettingsSection, LoginServiceCallback
    {
        #region Fields

        private bool _isLoggedIn;
        private LoginServiceClient _loginServiceClient;
        private string _userName;

        #endregion Fields

        #region Constructors

        public SpotifyLoginSection(ILoadingIndicatorService loadingIndicator)
        {
            LoadingIndicator = loadingIndicator;
            HeaderInfo = new HeaderInfo
            {
                Title = "Login details"
            };

            UI = new SpotifyLoginSectionView
            {
                Model = this
            };

            LogOutCommand = new AutomaticCommand(ExecuteLogOut, CanExecuteLogOut);
        }

        #endregion Constructors

        #region Properties

        public ILoadingIndicatorService LoadingIndicator
        {
            get; private set;
        }

        public HeaderInfo HeaderInfo
        {
            get; private set;
        }

        public object UI
        {
            get; private set;
        }

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    RaisePropertyChanged("IsLoggedIn");
                }
            }
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

        public AutomaticCommand LogOutCommand
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public void Load()
        {
            Task.Factory.StartNew(() =>
            {
                _loginServiceClient = new LoginServiceClient(new InstanceContext(this));

                try
                {
                    _loginServiceClient.Subscribe();

                    IsLoggedIn = _loginServiceClient.IsLoggedIn();
                    UserName = _loginServiceClient.GetRememberedUser();
                }
                catch (Exception)
                {
                    _loginServiceClient.Abort();
                }
            });
        }

        public void Save()
        {
            try
            {
                if (_loginServiceClient != null)
                {
                    _loginServiceClient.Close();
                }
            }
            catch (Exception e)
            {
                if (_loginServiceClient != null)
                {
                    _loginServiceClient.Abort();
                }
            }
        }

        void LoginServiceCallback.OnLoggedIn()
        {
            IsLoggedIn = true;
            LoadingIndicator.Pop();
        }

        void LoginServiceCallback.OnLoginError(string message)
        {
            IsLoggedIn = false;
            LoadingIndicator.Pop();
        }

        void LoginServiceCallback.OnLoggedOut()
        {
            IsLoggedIn = false;
            LoadingIndicator.Pop();
        }

        void LoginServiceCallback.OnPing()
        {
        }

        public void Login(string userName, string password)
        {
            if (_loginServiceClient != null)
            {
                try
                {
                    LoadingIndicator.Push();
                    _loginServiceClient.Login(userName, password, true);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private bool CanExecuteLogOut()
        {
            return IsLoggedIn;
        }

        private void ExecuteLogOut()
        {
        }

        #endregion Methods
    }
}