using System.Windows;

namespace Torshify.Radio
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            RadioBootstrapper bootstrapper = new RadioBootstrapper();
            bootstrapper.Run();
        }
    }
}