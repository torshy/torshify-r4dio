using System;
using System.ComponentModel.Composition;
using System.IO;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using Raven.Client;
using Raven.Client.Document;
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
            var documentConvention =
                new DocumentConvention
                {
                    FindTypeTagName =
                        type =>
                        {
                            if (typeof(Favorite).IsAssignableFrom(type))
                            {
                                return "favorites";
                            }

                            return DocumentConvention.DefaultTypeTagName(type);
                        },
                    FindClrTypeName =
                        type => type.AssemblyQualifiedName,
                    CustomizeJsonSerializer =
                        serializer =>
                        {
                            serializer.Binder = new CustomSerializationBinder();
                        }
                };

            _documentStore = new EmbeddableDocumentStore();
            _documentStore.Conventions = documentConvention;
            _documentStore.DataDirectory = Path.Combine(AppConstants.AppDataFolder, "Database");

            #if DEBUG
            _documentStore.UseEmbeddedHttpServer = true;
            #endif
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
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
            DocumentStore.Initialize();
        }

        private void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            if (DocumentStore != null)
            {
                DocumentStore.Dispose();
            }
        }

        #endregion Methods
    }
}