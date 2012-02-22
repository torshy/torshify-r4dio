using System;
using System.ComponentModel.Composition;
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
    //[ModuleExport(typeof(SpotifyModule), DependsOnModuleNames = new[] { "Core" })]
    public class SpotifyModule : MarshalByRefObject, IModule
    {
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
            //TileService.Add<MainStationView>(new TileData
            //                                 {
            //                                     Title = "Spotify",
            //                                     BackgroundImage =
            //                                         new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0003_Favs1.png")
            //                                 });

            if (!SpotifyAppDomainHandler.Instance.IsLoaded)
            {
                SpotifyAppDomainHandler.Instance.Load();
            }

            Login();
        }

        private void Login()
        {
            Task.Factory.StartNew(() =>
                                  {
                                      var client = new LoginServiceClient(new InstanceContext(new NoOpLoginCallback(ToastService)));

                                      try
                                      {
                                          client.Subscribe();

                                          var remembered = client.GetRememberedUser();

                                          if (!string.IsNullOrEmpty(remembered))
                                          {
                                              client.Relogin();
                                          }
                                      }
                                      catch (Exception e)
                                      {
                                          client.Abort();

                                          Thread.Sleep(1000);
                                          Login();
                                      }
                                  });
        }

        #endregion Methods

        #region Nested Types

        private class NoOpLoginCallback : LoginServiceCallback
        {
            private readonly IToastService _toastService;

            public NoOpLoginCallback(IToastService toastService)
            {
                _toastService = toastService;
            }

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