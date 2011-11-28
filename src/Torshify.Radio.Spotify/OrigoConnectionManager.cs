using System;
using Torshify.Origo.Host;

namespace Torshify.Radio.Spotify
{
    public class OrigoConnectionManager
    {
        #region Fields

        private static readonly OrigoConnectionManager _instance = new OrigoConnectionManager();

        private bool _initialized;

        #endregion Fields

        #region Constructors

        private OrigoConnectionManager()
        {
            Initialize();
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

        private void Initialize()
        {
            try
            {
                if (!_initialized)
                {
                    AppDomainSetup origoSetup = new AppDomainSetup();
                    origoSetup.ApplicationBase = Environment.CurrentDirectory;

                    AppDomain origoDomain = AppDomain.CreateDomain("OrigoDomain", null, origoSetup);

                    OrigoBootstrapperWrapper host = origoDomain.CreateInstanceAndUnwrap(
                        typeof (OrigoBootstrapperWrapper).Assembly.FullName,
                        "Torshify.Radio.Spotify.OrigoBootstrapperWrapper") as OrigoBootstrapperWrapper;

                    if (host != null)
                    {
                        InitializeCommandLineOptions(Environment.GetCommandLineArgs(), host);
                        host.Start();
                    }

                    _initialized = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitializeCommandLineOptions(string[] args, OrigoBootstrapperWrapper bootstrapper)
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