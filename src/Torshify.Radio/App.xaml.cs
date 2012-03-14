using System;
using System.Collections.Generic;
using System.Windows;

using Torshify.Radio.Utilities;

namespace Torshify.Radio
{
    public partial class App : Application, ISingleInstanceApp
    {
        #region Fields

        private const string ApplicationID = "Torshify.R4dio";
        private Bootstrapper _bootstrapper;

        #endregion Fields

        #region Methods

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(ApplicationID))
            {
#if !DEBUG
                SplashScreen splash = new SplashScreen("resources\\splashScreen.png");
                splash.Show(true, true);
#endif

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

            if (_bootstrapper != null)
            {
                _bootstrapper.HandleCommandLineArguments(args);
            }

            return true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _bootstrapper = new Bootstrapper();
            _bootstrapper.Run();
        }

        #endregion Methods
    }
}