using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.EchoNest.Services;
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
            RegionManager.RegisterViewWithRegion(Views.Browse.MainStationView.TabViewRegion, typeof(Views.Browse.Tabs.SearchResultsView));

            TileService.Add<Views.Browse.MainStationView>(new TileData
            {
                Title = "Artist browser",
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0029_programs.png")
            });

            SearchBarService.Add<Views.Browse.MainStationView>(new SearchBarData
            {
                Category = "Search for song or artist",
                WatermarkText = "Search for song or artist",
                AutoCompleteProvider = SuggestArtists
            });

            RegionManager.RegisterViewWithRegion(Views.Similar.MainStationView.TabViewRegion, typeof(Views.Similar.Tabs.SimilarView));
            RegionManager.RegisterViewWithRegion(Views.Similar.MainStationView.TabViewRegion, typeof(Views.Similar.Tabs.RecentView));

            TileService.Add<Views.Similar.MainStationView>(new TileData
            {
                Title = "Similar artists",
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MS_0000s_0031_net3.png")
            });

            SearchBarService.Add<Views.Similar.MainStationView>(new SearchBarData
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