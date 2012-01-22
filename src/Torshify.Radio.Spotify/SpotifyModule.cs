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

        #region Methods

        public void Initialize()
        {
            TileService.Create<SpotifyStation>(new TileData { Title = "Spotify" }, Tuple.Create("Biff", "Woff"));
            TileService.Create<SpotifyStation>(new TileData { Title = "Spotify 2" }, Tuple.Create("Hei", "1"));
            TileService.Create<MainStationView>(new TileData { Title = "Main Spotify View" }, Tuple.Create("HAHAH", "FETT"));
            TileService.Create<SpotifyStation>(new TileData { Title = "Spotify 4" });
        }

        #endregion Methods
    }

    [Export(typeof(SpotifyStation))]
    public class SpotifyStation : IRadioStation
    {
        [Import]
        public IRadio Radio
        {
            get;
            set;
        }

        public void OnTuneIn()
        {
        }

        public void OnTuneAway()
        {
        }
    }
}