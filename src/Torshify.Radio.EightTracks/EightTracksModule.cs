using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Threading;

using EightTracks;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.EightTracks.Views;
using Torshify.Radio.EightTracks.Views.Tabs;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks
{
    [ModuleExport(typeof(EightTracksModule), DependsOnModuleNames = new[] { "Core" })]
    public class EightTracksModule : IModule
    {
        #region Fields

        internal const string ApiKey = "63b5cb8daf03ec1df8f1c25fec5479b612739a29";

        private readonly Dispatcher _dispatcher;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public EightTracksModule(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        #endregion Constructors

        #region Properties

        [Import]
        public ILoggerFacade Logger
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
        public ITileService TileService
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
                RegionManager.RegisterViewWithRegion(MainStationView.TabViewRegion, typeof (MainTabView));
                RegionManager.RegisterViewWithRegion(MainStationView.TabViewRegion, typeof (RecentMixListView));
                RegionManager.RegisterViewWithRegion(MainStationView.TabViewRegion, typeof (PopularMixListView));
                RegionManager.RegisterViewWithRegion(MainStationView.TabViewRegion, typeof (HotMixListView));
                RegionManager.RegisterViewWithRegion(MainStationView.TabViewRegion, typeof (TagsTabView));
                RegionManager.RegisterViewWithRegion(MainStationView.TabViewRegion, typeof (FavoritesView));

                TileService.Add<MainStationView>(new TileData
                {
                    Title = "8tracks",
                    BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_9999_8tracks.png")
                });

                SearchBarService.Add<MainStationView>(new SearchBarData
                {
                    Category = "8tracks by tag",
                    WatermarkText = "Search for mixes by tag",
                    AutoCompleteProvider = GetMixesByTag
                },
                Tuple.Create("Type", "Tag"));

                SearchBarService.Add<MainStationView>(new SearchBarData
                {
                    Category = "8tracks by artist",
                    WatermarkText = "Search for mixes by artist",
                    AutoCompleteProvider = GetMixesByTag
                },
                Tuple.Create("Type", "Artist"));

            }), DispatcherPriority.Background);
        }

        private IEnumerable<string> GetMixesByTag(string tag)
        {
            try
            {
                using (var session = new EightTracksSession(ApiKey))
                {
                    TagsResponse response = session.Query<Tags>().Execute(1, tag);
                    return response.Tags.Select(t => t.Name);
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error while getting 8tracks mixes. " + e.Message, Category.Exception, Priority.Medium);
            }

            return new string[0];
        }

        #endregion Methods
    }
}