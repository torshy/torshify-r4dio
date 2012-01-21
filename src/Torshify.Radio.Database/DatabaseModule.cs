using System.ComponentModel.Composition;
using System.IO;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using Raven.Client;
using Raven.Client.Embedded;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Database
{
    [ModuleExport("Database", typeof(DatabaseModule))]
    public class DatabaseModule : IModule
    {
        #region Fields

        private EmbeddableDocumentStore _documentStore;

        #endregion Fields

        #region Constructors

        public DatabaseModule()
        {
            _documentStore = new EmbeddableDocumentStore();
            _documentStore.DataDirectory = Path.Combine(AppConstants.AppDataFolder, "Database");
        }

        #endregion Constructors

        #region Properties

        [Export]
        public IDocumentStore DocumentStore
        {
            get { return _documentStore; }
        }

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            DocumentStore.Initialize();
        }

        #endregion Methods
    }
}