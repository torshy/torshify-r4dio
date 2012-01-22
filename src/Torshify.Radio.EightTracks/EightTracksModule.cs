using System;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using Torshify.Radio.EightTracks.Views;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks
{
    [ModuleExport(typeof(EightTracksModule), DependsOnModuleNames = new[] { "Core" })]
    public class EightTracksModule : IModule
    {
        #region Properties

        [Import]
        public ITileService TileService
        {
            get;
            set;
        }

        [Import]
        public ISearchBarService SearchBarService
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
                                                    Title = "8tracks"
                                                });

            SearchBarService.Add<MainStationView>(new SearchBarData
                                                 {
                                                     Category = "8tracks by tag",
                                                     WatermarkText = "Search for mixes by tag"
                                                 },
                                                 Tuple.Create("Type", "Tag"));

            SearchBarService.Add<MainStationView>(new SearchBarData
                                                  {
                                                      Category = "8tracks by artist",
                                                      WatermarkText = "Search for mixes by artist"
                                                  },
                                                  Tuple.Create("Type", "Artist"));
        }

        #endregion Methods
    }
}