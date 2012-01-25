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
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0003_Favs1.png")
            },
            Tuple.Create("Argument1", "Value1"));

            TileService.Add<MainStationView>(new TileData
            {
                Title = "Spotify 2",
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0009_listen.png")
            },
            Tuple.Create("Argument1", "Value1"));

            TileService.Add<MainStationView>(new TileData
            {
                Title = "Main Spotify View",
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0014_msg3.png")
            });

            TileService.Add<MainStationView>(new TileData
            {
                Title = "Spotify 4",
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0015_voicemaill.png")
            });

            TileService.Add<MainStationView>(new TileData
            {
                Title = "Spotify",
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0019_profiles.png")
            },
            Tuple.Create("Argument1", "Value1"));

            TileService.Add<MainStationView>(new TileData
            {
                Title = "Spotify 2",
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0029_programs.png")
            },
            Tuple.Create("Argument1", "Value1"));

            TileService.Add<MainStationView>(new TileData
            {
                Title = "Main Spotify View",
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_9999_8tracks.png")
            });

            TileService.Add<MainStationView>(new TileData
            {
                Title = "Spotify 4",
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MS_0000s_0031_net3.png")
            });
        }

        #endregion Methods
    }
}