using System;
using System.IO;
using System.Reflection;
using Torshify.Origo;
using Torshify.Origo.Host;

namespace Torshify.Radio.Spotify
{
    public class OrigoConnectionManager : MarshalByRefObject
    {
        #region Fields

        private static readonly OrigoConnectionManager _instance = new OrigoConnectionManager();

        private bool _initialized;

        #endregion Fields

        #region Constructors

        private OrigoConnectionManager()
        {
        }

        #endregion Constructors

        #region Properties

        public static OrigoConnectionManager Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            try
            {
                if (!_initialized)
                {
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>");
                    _initialized = true;

                    AppDomainSetup setup = new AppDomainSetup();
                    setup.ApplicationName = "Spotify";
                    setup.ApplicationBase = Environment.CurrentDirectory;
                    setup.PrivateBinPath = Path.Combine("Modules", "Spotify");
                    setup.PrivateBinPathProbe = "true";

                    AppDomain origoDomain = AppDomain.CreateDomain("OrigoDomain", null, setup);
                    origoDomain.UnhandledException += OrigoDomainOnUnhandledException;
                    AppDomain.CurrentDomain.AssemblyResolve += OrigoDomainOnAssemblyResolve;
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                    OrigoBootstrapper host = origoDomain.CreateInstanceAndUnwrap(
                        typeof(OrigoBootstrapper).Assembly.FullName,
                        "Torshify.Origo.OrigoBootstrapper") as OrigoBootstrapper;
                    AppDomain.CurrentDomain.AssemblyResolve -= OrigoDomainOnAssemblyResolve;

                    if (host != null)
                    {
                        InitializeCommandLineOptions(Environment.GetCommandLineArgs(), host);
                        host.Run();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
        }

        private void OrigoDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {

        }

        private Assembly OrigoDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                Assembly assembly = Assembly.Load(args.Name);
                if (assembly != null)
                    return assembly;
            }
            catch
            {
                // ignore load error 
            }

            // *** Try to load by filename - split out the filename of the full assembly name
            // *** and append the base path of the original assembly (ie. look in the same dir)
            // *** NOTE: this doesn't account for special search paths but then that never
            //           worked before either.
            string[] Parts = args.Name.Split(',');
            string File = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + Parts[0].Trim() +
                          ".dll";

            return System.Reflection.Assembly.LoadFrom(File);

        }

        private void InitializeCommandLineOptions(string[] args, OrigoBootstrapper bootstrapper)
        {
            bool showHelp = false;

            var p = new OptionSet
                        {
                            { "u|username=", "spotify username", userName => bootstrapper.UserName = userName },
                            { "p|password=", "spotify password", password => bootstrapper.Password = password },
                            { "httpPort=", "the port the http wcf services will be hosted on", (int port) => bootstrapper.HttpPort = port },
                            { "tcpPort=", "the port the tcp wcf services will be hosted on", (int port) => bootstrapper.TcpPort = port },
                            { "h|help",  "show this message and exit", v => showHelp = v != null }
                        };

            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
            }

            if (showHelp)
            {
                p.WriteOptionDescriptions(Console.Out);
                Console.ReadLine();
                Environment.Exit(0);
            }
        }


        #endregion Methods
    }
}