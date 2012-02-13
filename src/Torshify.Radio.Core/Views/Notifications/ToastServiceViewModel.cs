using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Threading;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Views.Notifications
{
    [Export]
    public class ToastServiceViewModel : NotificationObject
    {
        #region Fields

        private ToastData _currentToast;
        private object _lock = new object();
        private ConcurrentQueue<ToastData> _toastQueue;
        private Thread _toastThread;

        #endregion Fields

        #region Constructors

        public ToastServiceViewModel()
        {
            _toastQueue = new ConcurrentQueue<ToastData>();
            _toastThread = new Thread(ToastRun);
            _toastThread.IsBackground = true;
            _toastThread.Start();
        }

        #endregion Constructors

        #region Events

        public event EventHandler Activate;

        public event EventHandler Deactivate;

        #endregion Events

        #region Properties

        public ToastData CurrentToast
        {
            get
            {
                return _currentToast;
            }
            set
            {
                if (_currentToast != value)
                {
                    _currentToast = value;
                    RaisePropertyChanged("CurrentToast");
                }
            }
        }

        #endregion Properties

        #region Methods

        public void NewNotification(ToastData data)
        {
            lock (_lock)
            {
                _toastQueue.Enqueue(data);
                Monitor.Pulse(_lock);
            }
        }

        private void OnActivate()
        {
            EventHandler handler = Activate;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnDeactivate()
        {
            EventHandler handler = Deactivate;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void ToastRun()
        {
            while (true)
            {
                lock (_lock)
                {
                    if (_toastQueue.IsEmpty)
                    {
                        if (CurrentToast != null)
                        {
                            CurrentToast = null;
                            OnDeactivate();
                        }

                        Monitor.Wait(_lock);
                    }
                }

                ToastData toastData;
                if (_toastQueue.TryDequeue(out toastData))
                {
                    OnActivate();
                    CurrentToast = toastData;
                    Thread.Sleep(Math.Max(toastData.DisplayTime, 2000));
                }
            }
        }

        #endregion Methods
    }
}