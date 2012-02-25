using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.Framework;
using Torshify.Radio.Spotify.LoginService;

namespace Torshify.Radio.Spotify
{
    [ModuleExport(typeof(SpotifyModule), DependsOnModuleNames = new[] { "Core" })]
    public class SpotifyModule : MarshalByRefObject, IModule
    {
        #region Fields

        private Process _orgioProcess;

        #endregion Fields

        #region Properties

        [Import]
        public IRegionManager RegionManager
        {
            get;
            set;
        }

        [Import]
        public ITileService TileService
        {
            get;
            set;
        }

        [Import]
        public IToastService ToastService
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void Initialize()
        {
#if !DEBUG
            var processes = Process.GetProcesses().Where(p => p.ProcessName.StartsWith("Torshify.Origo.Host"));

            if (!processes.Any())
            {
                string thisAssemblyLocation = GetType().Assembly.Location;
                string thisAssemblyDirectory = Path.GetDirectoryName(thisAssemblyLocation);
                string origoExe = "Torshify.Origo.Host.exe";
                string origoPath = Path.Combine(thisAssemblyDirectory, origoExe);

                if (File.Exists(origoPath))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(origoPath);
                    startInfo.WorkingDirectory = thisAssemblyDirectory;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    _orgioProcess = Process.Start(startInfo);
                    _orgioProcess.Refresh();
                    AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
                }
            }
            else
            {
                _orgioProcess = processes.FirstOrDefault();
            }
#endif

            Login();
        }

        private void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            if (_orgioProcess != null)
            {
                if (!_orgioProcess.HasExited)
                {
                    try
                    {
                        _orgioProcess.Kill();
                        _orgioProcess.Dispose();
                        _orgioProcess = null;
                    }
                    catch (Exception)
                    {
                        // Ignore
                    }
                }
            }
        }

        private void Login()
        {
            Task.Factory.StartNew(() =>
            {
                var client = new LoginServiceClient(new InstanceContext(new NoOpLoginCallback(ToastService)));

                try
                {
                    client.Subscribe();

                    if (!client.IsLoggedIn())
                    {
                        var remembered = client.GetRememberedUser();

                        if (!string.IsNullOrEmpty(remembered))
                        {
                            client.Relogin();
                        }
                    }
                }
                catch (Exception e)
                {
                    client.Abort();
                    Thread.Sleep(2000);
                    Login();
                }
            });
        }

        #endregion Methods

        #region Nested Types

        private class NoOpLoginCallback : LoginServiceCallback
        {
            #region Fields

            private readonly IToastService _toastService;

            #endregion Fields

            #region Constructors

            public NoOpLoginCallback(IToastService toastService)
            {
                _toastService = toastService;
            }

            #endregion Constructors

            #region Methods

            public void OnLoggedIn()
            {
                _toastService.Show("Spotify: Logged in");
            }

            public void OnLoginError(string message)
            {
                _toastService.Show("Spotify: " + message);
            }

            public void OnLoggedOut()
            {
                _toastService.Show("Spotify: Logged out");
            }

            public void OnPing()
            {
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}