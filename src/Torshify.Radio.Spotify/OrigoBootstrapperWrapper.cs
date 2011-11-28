using System;
using Microsoft.Practices.ServiceLocation;
using Torshify.Origo;
using Torshify.Origo.Host;
using Torshify.Origo.Extensions;

namespace Torshify.Radio.Spotify
{
    public class OrigoBootstrapperWrapper : MarshalByRefObject
    {
        #region Fields

        private OrigoBootstrapper _bootstrapper;

        public string UserName { get; set; }

        public string Password { get; set; }

        public int HttpPort { get; set; }

        public int TcpPort { get; set; }

        #endregion Fields

        #region Methods

        public void Start()
        {
            try
            {
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                
                    _bootstrapper = new OrigoBootstrapper();
                    _bootstrapper.UserName = UserName;
                    _bootstrapper.Password = Password;
                    _bootstrapper.Run();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            
        }

        private void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            try
            {
                var session = ServiceLocator.Current.TryResolve<ISession>();

                if (session != null)
                {
                    session.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion Methods
    }
}