using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using Raven.Client;

namespace Torshify.Radio.Spotify
{
    [ModuleExport(typeof(SpotifyModule), DependsOnModuleNames = new[] { "Database" })]
    public class SpotifyModule : IModule
    {
        #region Properties

        [Import]
        public IDocumentStore DocumentStore
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void Initialize()
        {

        }

        #endregion Methods
    }
}