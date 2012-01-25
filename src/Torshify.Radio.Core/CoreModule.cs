using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.Core.Views;
using Torshify.Radio.Core.Views.Controls;
using Torshify.Radio.Core.Views.NowPlaying;
using Torshify.Radio.Core.Views.Stations;
using Torshify.Radio.Framework;

using WPFLocalizeExtension.Engine;

namespace Torshify.Radio.Core
{
    [ModuleExport("Core", typeof(CoreModule), DependsOnModuleNames = new[] { "Database" })]
    public class CoreModule : IModule
    {
        #region Properties

        [Import]
        public IRegionManager RegionManager
        {
            get;
            set;
        }

        [ImportMany]
        public IEnumerable<IStartable> Startables
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            foreach (var startable in Startables)
            {
                startable.Start();
            }

            // TODO : Load locale from db
            LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("en-US");

            RegionManager.RegisterViewWithRegion(AppRegions.MainRegion, typeof (MainView));
            RegionManager.RegisterViewWithRegion(AppRegions.BottomRegion, typeof(ControlsView));
            RegionManager.RegisterViewWithRegion(AppRegions.ViewRegion, typeof (StationsView));

            RegionManager.RequestNavigate(AppRegions.MainRegion, typeof(MainView).FullName);
            RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof(StationsView).FullName);
        }

        #endregion Methods
    }
}