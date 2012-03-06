using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

using Hardcodet.Scheduling;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;
using Torshify.Radio.Framework.Controls;
using Torshify.Radio.Logging;
using Torshify.Radio.Regions;
using Torshify.Radio.Utilities;

namespace Torshify.Radio
{
    public class Bootstrapper : MefBootstrapper
    {
        #region Constructors

        public Bootstrapper()
        {
            Scheduler = new Scheduler();
        }

        #endregion Constructors

        #region Properties

        [Export]
        public Dispatcher Dispatcher
        {
            get { return Application.Current.Dispatcher; }
        }

        [Export]
        public Scheduler Scheduler
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
            AggregateCatalog.Catalogs.Add(new DirectoryCatalog(Environment.CurrentDirectory, "Torshify.Radio*.dll"));

            Directory.CreateDirectory("Modules");
            var modules = Directory.EnumerateDirectories(Path.Combine(Environment.CurrentDirectory, "Modules"));
            foreach (var module in modules)
            {
                AggregateCatalog.Catalogs.Add(new DirectoryCatalog(module, "Torshify.Radio*.dll"));
            }
        }

        protected override ILoggerFacade CreateLogger()
        {
            InitializeLogging();
            return new Log4NetFacade();
        }

        protected override DependencyObject CreateShell()
        {
            Timeline
                .DesiredFrameRateProperty
                .OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 30 });

            return Container.GetExportedValue<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow = (Shell)Shell;
            Application.Current.MainWindow.InputBindings.Add(new KeyBinding(new StaticCommand(ConsoleManager.Toggle), Key.D0, ModifierKeys.Alt));
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings mappings = base.ConfigureRegionAdapterMappings();
            mappings.RegisterMapping(typeof(TransitioningContentControl), ServiceLocator.Current.GetInstance<TransitionContentControlRegionAdapter>());
            return mappings;
        }

        private void InitializeLogging()
        {
            var fileAppender = new RollingFileAppender();
            fileAppender.File = Path.Combine(AppConstants.LogFolder, "Torshify.Radio.log");
            fileAppender.AppendToFile = true;
            fileAppender.MaxSizeRollBackups = 10;
            fileAppender.MaxFileSize = 1024 * 1024;
            fileAppender.RollingStyle = RollingFileAppender.RollingMode.Size;
            fileAppender.StaticLogFileName = true;
            fileAppender.Layout = new PatternLayout("%date{dd MMM yyyy HH:mm} [%thread] %-5level %logger - %message%newline");
            fileAppender.Threshold = Level.Info;
            fileAppender.ActivateOptions();

            var consoleAppender = new CustomConsoleColorAppender();
            consoleAppender.AddMapping(
                new CustomConsoleColorAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity,
                    BackColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Fatal
                });
            consoleAppender.AddMapping(
                new CustomConsoleColorAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Error
                });
            consoleAppender.AddMapping(
                new CustomConsoleColorAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.Yellow | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Warn
                });
            consoleAppender.AddMapping(
                new CustomConsoleColorAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.Green | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Info
                });
            consoleAppender.AddMapping(
                new CustomConsoleColorAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Info
                });
            consoleAppender.Layout = new PatternLayout("%date{dd MM HH:mm} %-5level - %message%newline");
            consoleAppender.Threshold = Level.All;
            consoleAppender.ActivateOptions();

            Logger root;
            root = ((Hierarchy)LogManager.GetRepository()).Root;
            root.AddAppender(consoleAppender);
            root.AddAppender(fileAppender);
            root.Repository.Configured = true;

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Exception exception = (Exception)e.ExceptionObject;
                Logger.Log(exception.ToString(), Category.Exception, Priority.High);
            };

            Application.Current.Dispatcher.UnhandledException += (s, e) =>
            {
                Exception exception = e.Exception;
                Logger.Log(exception.ToString(), Category.Exception, Priority.High);

                // Some images throw this exception. Lets try and handle it to prevent the application from crashing
                if (exception is NotSupportedException || exception is AccessViolationException)
                {
                    e.Handled = true;
                }
            };
        }

        #endregion Methods
    }
}