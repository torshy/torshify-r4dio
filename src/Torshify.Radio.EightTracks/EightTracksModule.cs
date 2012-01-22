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

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            TileService.Create<MainStationView>(new TileData
                                                {
                                                    Title = "8tracks"
                                                });
        }

        #endregion Methods
    }
}