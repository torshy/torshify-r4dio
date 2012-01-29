using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Torshify.Radio.Framework;
using Torshify.Radio.Spotify.Views;

namespace Torshify.Radio.Spotify
{
    [ModuleExport(typeof(SpotifyModule), DependsOnModuleNames = new[] { "Core" })]
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

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            TileService.Add<MainStationView>(new TileData
                                             {
                                                 Title = "Spotify",
                                                 BackgroundImage =
                                                     new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0003_Favs1.png")
                                             });

            if (!SpotifyAppDomainHandler.Instance.IsLoaded)
            {
                SpotifyAppDomainHandler.Instance.Load();
            }
        }

        #endregion Methods
    }
}