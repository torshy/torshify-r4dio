using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Torshify.Origo;

namespace Torshify.Radio.Spotify
{
    public class SpotifyAppDomainHandler
    {
        #region Fields

        private static readonly SpotifyAppDomainHandler _instance = new SpotifyAppDomainHandler();

        private AppDomain _appDomain;
        private object _lock = new object();

        #endregion Fields

        #region Constructors

        static SpotifyAppDomainHandler()
        {
        }

        #endregion Constructors

        #region Properties

        public static SpotifyAppDomainHandler Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool IsLoaded
        {
            get { return _appDomain != null; }
        }

        #endregion Properties

        #region Methods

        public void Load()
        {
            if (!IsLoaded)
            {
                lock (_lock)
                {
                    try
                    {
                        if (!IsLoaded)
                        {
                            StartOrigo();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public void Unload()
        {
            if (_appDomain != null)
            {
                AppDomain.Unload(_appDomain);
                _appDomain = null;
            }
        }

        private static bool AssemblyNameIsSatelliteAssembly(string assemblyName)
        {
            AssemblyName name = new AssemblyName(assemblyName);
            if (name.Name.EndsWith(".resources") && name.CultureInfo.Name.Length > 0)
            {
                return true;
            }

            return false;
        }

        private void StartOrigo()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "Spotify";
            setup.ApplicationBase = Environment.CurrentDirectory;
            setup.PrivateBinPath = Path.Combine("Modules", "Spotify");
            setup.PrivateBinPathProbe = "true";

            _appDomain = AppDomain.CreateDomain("OrigoDomain", null, setup);
            AppDomain.CurrentDomain.AssemblyResolve += OrigoDomainOnAssemblyResolve;
            OrigoBootstrapper host = _appDomain.CreateInstanceAndUnwrap(
                typeof(OrigoBootstrapper).Assembly.FullName,
                "Torshify.Origo.OrigoBootstrapper") as OrigoBootstrapper;
            AppDomain.CurrentDomain.AssemblyResolve -= OrigoDomainOnAssemblyResolve;

            if (host != null)
            {
                InitializeCommandLineOptions(Environment.GetCommandLineArgs(), host);
                host.Run();
            }
            else
            {
                AppDomain.Unload(_appDomain);
                _appDomain = null;
            }
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
            }
        }

        private Assembly OrigoDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (AssemblyNameIsSatelliteAssembly(args.Name))
            {
                return args.RequestingAssembly;
            }

            try
            {
                Assembly assembly = Assembly.Load(args.Name);
                if (assembly != null)
                {
                    return assembly;
                }
            }
            catch
            {
                // ignore load error
            }

            // *** Try to load by filename - split out the filename of the full assembly name
            // *** and append the base path of the original assembly (ie. look in the same dir)
            // *** NOTE: this doesn't account for special search paths but then that never
            //           worked before either.
            string[] parts = args.Name.Split(',');
            string assemblyFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + parts[0].Trim() + ".dll";
            return Assembly.LoadFrom(assemblyFile);
        }

        #endregion Methods
    }
}