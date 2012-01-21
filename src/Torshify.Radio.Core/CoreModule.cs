using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using Raven.Client;

using Torshify.Radio.Core.Models;

namespace Torshify.Radio.Core
{
    [ModuleExport(typeof(CoreModule), DependsOnModuleNames = new[] { "Database" })]
    public class CoreModule : IModule
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
            var mainWindow = Application.Current.MainWindow;

            using(var session = DocumentStore.OpenSession())
            {
                var settings = session.Query<ShellSetting>().FirstOrDefault();

                if (settings != null)
                {
                    mainWindow.Width = settings.WindowWidth;
                    mainWindow.Height = settings.WindowHeight;
                }
            }

            mainWindow.Closing += MainWindowOnClosing;
            mainWindow.Show();
        }

        private void MainWindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var settings = session.Query<ShellSetting>().FirstOrDefault();

                if (settings == null)
                {
                    settings = new ShellSetting();
                }

                settings.WindowHeight = Application.Current.MainWindow.ActualHeight;
                settings.WindowWidth = Application.Current.MainWindow.ActualWidth;
                
                session.Store(settings);
                session.SaveChanges();
            }
        }

        #endregion Methods
    }
}