using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.Core.Views;
using Torshify.Radio.Core.Views.Stations;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core
{
    [ModuleExport(typeof(CoreModule), DependsOnModuleNames = new[] { "Database" })]
    public class CoreModule : IModule
    {
        #region Properties

        [ImportMany]
        public IEnumerable<IStartable> Startables
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
            foreach (var startable in Startables)
            {
                startable.Start();
            }

            RegionManager.RegisterViewWithRegion(AppRegions.MainRegion, typeof (MainView));
            RegionManager.RegisterViewWithRegion(AppRegions.ViewRegion, typeof (StationsView));
        }

        #endregion Methods
    }
}