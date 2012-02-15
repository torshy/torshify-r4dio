using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Threading;

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
        #region Fields

        internal const string ApiKey = "RJOXXESVUVZ07WY1T";

        private readonly Dispatcher _dispatcher;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public EchoNestModule(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        #endregion Constructors

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

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            _dispatcher.BeginInvoke(new Action(() =>
            {
                RegionManager.RegisterViewWithRegion(AppRegions.ViewRegion, typeof(Views.Browse.Tabs.AlbumView));
                RegionManager.RegisterViewWithRegion(AppRegions.ViewRegion, typeof(Views.Browse.Tabs.ArtistView));

                RegionManager.RegisterViewWithRegion(Views.Browse.MainStationView.TabViewRegion,
                                                     typeof(Views.Browse.Tabs.SearchResultsView));

                TileService.Add<Views.Browse.MainStationView>(new TileData
                {
                    Title = "Search",
                    BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0029_programs.png")
                });

                SearchBarService.Add<Views.Browse.MainStationView>(new SearchBarData
                {
                    Category = "Search_For_Song_Or_Artist",
                    WatermarkText = "Search_For_Song_Or_Artist",
                    AutoCompleteProvider = SuggestArtists
                });

                SearchBarService.SetActive(
                    sbar => sbar.NavigationUri.OriginalString == typeof(Views.Browse.MainStationView).FullName);
            }),
            DispatcherPriority.Background);

            _dispatcher.BeginInvoke(new Action(() =>
            {
                RegionManager.RegisterViewWithRegion(Views.Similar.MainStationView.TabViewRegion,
                                                     typeof(Views.Similar.Tabs.SimilarView));
                RegionManager.RegisterViewWithRegion(Views.Similar.MainStationView.TabViewRegion,
                                                     typeof(Views.Similar.Tabs.RecentView));

                TileService.Add<Views.Similar.MainStationView>(new TileData
                {
                    Title = "Similar artists",
                    BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MS_0000s_0031_net3.png")
                });

                SearchBarService.Add<Views.Similar.MainStationView>(new SearchBarData
                {
                    Category = "Search_For_Similar_Artist",
                    WatermarkText = "Search_For_Similar_Artist_Watermark",
                    AutoCompleteProvider = SuggestArtists
                });

            }),
            DispatcherPriority.Background);

            _dispatcher.BeginInvoke(new Action(() =>
            {
                RegionManager.RegisterViewWithRegion(Views.Favorites.MainStationView.TabViewRegion,
                                        typeof(Views.Favorites.Tabs.FavoritesView));

                TileService.Add<Views.Favorites.MainStationView>(new TileData
                {
                    Title = "Favorites",
                    BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0004_favs2.png")
                });
            }),
            DispatcherPriority.Background);
        }

        private IEnumerable<string> SuggestArtists(string query)
        {
            try
            {
                return SuggestArtistsService.GetSimilarArtists(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return new string[0];
        }

        #endregion Methods
    }
}