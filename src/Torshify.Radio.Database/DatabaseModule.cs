using System.IO;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using Raven.Client.Embedded;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Database
{
    [ModuleExport("Database", typeof(DatabaseModule))]
    public class DatabaseModule : IModule
    {
        #region Methods

        public void Initialize()
        {
            var documentStore = new EmbeddableDocumentStore
            {
                DataDirectory = Path.Combine(AppConstants.AppDataFolder, "Database")
            };

            documentStore.Initialize();
        }

        #endregion Methods
    }
}