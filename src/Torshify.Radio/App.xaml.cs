using System;
using System.Collections.Generic;
using System.Windows;

using Microsoft.Shell;

namespace Torshify.Radio
{
    public partial class App : Application, ISingleInstanceApp
    {
        #region Fields

        private const string Unique = "Torshify.R4dio";

        #endregion Fields

        #region Methods

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                SingleInstance<App>.Cleanup();
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (MainWindow.WindowState == WindowState.Minimized)
            {
                MainWindow.WindowState = WindowState.Normal;
            }

            MainWindow.Activate();

            return true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            RadioBootstrapper bootstrapper = new RadioBootstrapper();
            bootstrapper.Run();
        }

        #endregion Methods
    }
}