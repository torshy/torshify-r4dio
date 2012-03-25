using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.Framework;
using Torshify.Radio.Spotify.LoginService;

namespace Torshify.Radio.Spotify
{
    [ModuleExport(typeof(SpotifyModule), DependsOnModuleNames = new[] { "Core" })]
    public class SpotifyModule : MarshalByRefObject, IModule
    {
        #region Fields

        private Process _orgioProcess;

        #endregion Fields

        #region Properties

        [Import]
        public IRegionManager RegionManager
        {
            get;
            set;
        }

        [Import]
        public ITileService TileService
        {
            get;
            set;
        }

        [Import]
        public IToastService ToastService
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            #if !DEBUG
            var processes = Process.GetProcesses().Where(p => p.ProcessName.StartsWith("Torshify.Origo.Host"));

            if (!processes.Any())
            {
                string thisAssemblyLocation = GetType().Assembly.Location;
                string thisAssemblyDirectory = Path.GetDirectoryName(thisAssemblyLocation);
                string origoExe = "Torshify.Origo.Host.exe";
                string origoPath = Path.Combine(thisAssemblyDirectory, origoExe);

                if (File.Exists(origoPath))
                {
                    const uint NORMAL_PRIORITY_CLASS = 0x0020;

                    bool retValue;
                    string Application = origoPath;
                    PROCESS_INFORMATION pInfo = new PROCESS_INFORMATION();
                    STARTUPINFO sInfo = new STARTUPINFO();
                    sInfo.wShowWindow = 0;
                    sInfo.dwFlags = 0x00000001;
                    SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();
                    SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();
                    pSec.nLength = Marshal.SizeOf(pSec);
                    tSec.nLength = Marshal.SizeOf(tSec);

                    //Open Notepad
                    retValue = CreateProcess(Application, string.Empty,
                        ref pSec, ref tSec, false, NORMAL_PRIORITY_CLASS,
                        IntPtr.Zero, null, ref sInfo, out pInfo);

                    //ProcessStartInfo startInfo = new ProcessStartInfo(origoPath);
                    //startInfo.WorkingDirectory = thisAssemblyDirectory;
                    //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //_orgioProcess = Process.Start(startInfo);
                    //_orgioProcess.Refresh();
                    _orgioProcess = Process.GetProcessById(pInfo.dwProcessId);
                    AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
                }
            }
            else
            {
                _orgioProcess = processes.FirstOrDefault();
            }
            #endif

            Login();
        }

        [DllImport("kernel32.dll")]
        static extern bool CreateProcess(string lpApplicationName,
            string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles,
            uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
            [In] ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        private void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            if (_orgioProcess != null)
            {
                if (!_orgioProcess.HasExited)
                {
                    try
                    {
                        _orgioProcess.Kill();
                        _orgioProcess.Dispose();
                        _orgioProcess = null;
                    }
                    catch (Exception)
                    {
                        // Ignore
                    }
                }
            }
        }

        private void Login()
        {
            Task.Factory.StartNew(() =>
            {
                var client = new LoginServiceClient(new InstanceContext(new NoOpLoginCallback(ToastService)));

                try
                {
                    client.Subscribe();

                    if (!client.IsLoggedIn())
                    {
                        var remembered = client.GetRememberedUser();

                        if (!string.IsNullOrEmpty(remembered))
                        {
                            client.Relogin();
                        }
                    }
                }
                catch (Exception e)
                {
                    client.Abort();
                    Thread.Sleep(2000);
                    Login();
                }
            });
        }

        #endregion Methods

        #region Nested Types

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        private class NoOpLoginCallback : LoginServiceCallback
        {
            #region Fields

            private readonly IToastService _toastService;

            #endregion Fields

            #region Constructors

            public NoOpLoginCallback(IToastService toastService)
            {
                _toastService = toastService;
            }

            #endregion Constructors

            #region Methods

            public void OnLoggedIn()
            {
                _toastService.Show("Spotify: Logged in");
            }

            public void OnLoginError(string message)
            {
                _toastService.Show("Spotify: " + message);
            }

            public void OnLoggedOut()
            {
                _toastService.Show("Spotify: Logged out");
            }

            public void OnPing()
            {
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}