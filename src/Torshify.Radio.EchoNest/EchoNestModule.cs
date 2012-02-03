using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Torshify.Radio.EchoNest.Services;
using Torshify.Radio.EchoNest.Views.Similar;
using Torshify.Radio.EchoNest.Views.Similar.Tabs;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest
{
    [ModuleExport(typeof(EchoNestModule), DependsOnModuleNames = new[] { "Core" })]
    public class EchoNestModule : IModule
    {
        internal const string ApiKey = "RJOXXESVUVZ07WY1T";

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

        [Import]
        public ISuggestArtistsService SuggestArtistsService
        {
            get; 
            set;
        }

        [Import]
        public IRegionManager RegionManager
        {
            get;
            set;
        }

        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion(MainStationView.TabViewRegion, typeof(SimilarView));

            TileService.Add<MainStationView>(new TileData
            {
                Title = "Similar artists",
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0029_programs.png")
            });

            SearchBarService.Add<MainStationView>(new SearchBarData
            {
                Category = "Similar artists",
                WatermarkText = "Find similar artists",
                AutoCompleteProvider = SuggestArtists
            });
        }

        private IEnumerable<string> SuggestArtists(string query)
        {
            try
            {
                return SuggestArtistsService.GetSimilarArtists(query);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return new string[0];
        }
    }
}