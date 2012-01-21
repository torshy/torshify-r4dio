using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

using Raven.Client;
using Torshify.Radio.Core.Models;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core
{
    public class ShellSettingsHandler : IStartable
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

        void IStartable.Start()
        {
            var mainWindow = Application.Current.MainWindow;

            using (var session = DocumentStore.OpenSession())
            {
                var settings = session.Query<ShellSettings>().FirstOrDefault();

                if (settings != null)
                {
                    mainWindow.Width = settings.WindowWidth;
                    mainWindow.Height = settings.WindowHeight;
                    mainWindow.Left = settings.WindowLeft;
                    mainWindow.Top = settings.WindowTop;
                }
            }

            mainWindow.Closing += MainWindowOnClosing;
            mainWindow.Show();
        }

        private void MainWindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var settings = session.Query<ShellSettings>().FirstOrDefault();

                if (settings == null)
                {
                    settings = new ShellSettings();
                }

                var mainWindow = Application.Current.MainWindow;
                settings.WindowHeight = mainWindow.ActualHeight;
                settings.WindowWidth = mainWindow.ActualWidth;
                settings.WindowLeft = mainWindow.Left;
                settings.WindowTop = mainWindow.Top;
                session.Store(settings);
                session.SaveChanges();
            }
        }

        #endregion Methods
    }
}