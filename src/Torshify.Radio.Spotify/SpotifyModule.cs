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
    public class SpotifyModule : IModule
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
                                               },
                                               Tuple.Create("Argument1", "Value1"));

            TileService.Add<MainStationView>(new TileData
                                               {
                                                   Title = "Spotify 2",
                                               },
                                               Tuple.Create("Argument1", "Value1"));

            TileService.Add<MainStationView>(new TileData
                                                {
                                                    Title = "Main Spotify View",
                                                });

            TileService.Add<MainStationView>(new TileData
                                               {
                                                   Title = "Spotify 4"
                                               });
        }

        #endregion Methods
    }
}