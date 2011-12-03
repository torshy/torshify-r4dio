using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Input;

using Microsoft.Practices.Prism.Events;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Events;

namespace Torshify.Radio.Services
{
    [Export(typeof(IStartable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class InactivityNotificatorService : IStartable
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private bool _isApplicationInactive;
        private bool _isSystemInactive;
        private DateTime _lastAppicationInputActivity;
        private Timer _timer;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public InactivityNotificatorService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            _timer = new Timer();
            _timer.Interval = 200;
            _timer.Elapsed += OnInactivityCheckTick;
            _timer.Start();
            _lastAppicationInputActivity = DateTime.Now;
            InputManager.Current.PostProcessInput += OnPreProcess;
        }

        public void Stop()
        {
            _timer.Elapsed -= OnInactivityCheckTick;
            _timer.Stop();

            InputManager.Current.PostProcessInput -= OnPreProcess;
        }

        private void OnInactivityCheckTick(object sender, EventArgs e)
        {
            if (IdleTimeDetector.GetIdleTimeInfo().IdleTime > TimeSpan.FromSeconds(60))
            {
                if (!_isSystemInactive)
                {
                    _isSystemInactive = true;
                    PublishSystemActivityEvent();
                }
            }
            else
            {
                if (_isSystemInactive)
                {
                    _isSystemInactive = false;
                    PublishSystemActivityEvent();
                }
            }

            if (DateTime.Now.Subtract(_lastAppicationInputActivity) > TimeSpan.FromSeconds(10))
            {
                if (!_isApplicationInactive)
                {
                    _isApplicationInactive = true;
                    PublishApplicationActivityEvent();
                }
            }
            else
            {
                if (_isApplicationInactive)
                {
                    _isApplicationInactive = false;
                    PublishApplicationActivityEvent();
                }
            }
        }

        private void OnPreProcess(object sender, ProcessInputEventArgs e)
        {
            if (e.StagingItem.Input is KeyboardEventArgs ||
                e.StagingItem.Input is MouseEventArgs ||
                e.StagingItem.Input is TouchEventArgs)
            {
                _lastAppicationInputActivity = DateTime.Now;
            }
        }

        private void PublishApplicationActivityEvent()
        {
            _eventAggregator
                .GetEvent<ApplicationInactivityEvent>()
                .Publish(_isApplicationInactive);
        }

        private void PublishSystemActivityEvent()
        {
            _eventAggregator
                .GetEvent<SystemInactivityEvent>()
                .Publish(_isSystemInactive);
        }

        #endregion Methods

        #region Nested Types

        private struct LASTINPUTINFO
        {
            #region Fields

            public uint cbSize;
            public uint dwTime;

            #endregion Fields
        }

        private class IdleTimeDetector
        {
            #region Methods

            public static IdleTimeInfo GetIdleTimeInfo()
            {
                int systemUptime = Environment.TickCount,
                    lastInputTicks = 0,
                    idleTicks = 0;

                LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
                lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
                lastInputInfo.dwTime = 0;

                if (GetLastInputInfo(ref lastInputInfo))
                {
                    lastInputTicks = (int)lastInputInfo.dwTime;

                    idleTicks = systemUptime - lastInputTicks;
                }

                return new IdleTimeInfo
                {
                    LastInputTime = DateTime.Now.AddMilliseconds(-1 * idleTicks),
                    IdleTime = new TimeSpan(0, 0, 0, 0, idleTicks),
                    SystemUptimeMilliseconds = systemUptime,
                };
            }

            [DllImport("user32.dll")]
            static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

            #endregion Methods
        }

        private class IdleTimeInfo
        {
            #region Properties

            public TimeSpan IdleTime
            {
                get; internal set;
            }

            public DateTime LastInputTime
            {
                get; internal set;
            }

            public int SystemUptimeMilliseconds
            {
                get; internal set;
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}