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
        private ManualResetEventSlim _event;
        private ConcurrentQueue<ToastData> _toastQueue;
        private Thread _toastThread;

        #endregion Fields

        #region Constructors

        public ToastServiceViewModel()
        {
            _event = new ManualResetEventSlim(false);
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
            get { return _currentToast; }
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

        public void OnActivate()
        {
            EventHandler handler = Activate;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void OnDeactivate()
        {
            EventHandler handler = Deactivate;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void NewNotification(ToastData data)
        {
            _toastQueue.Enqueue(data);
            _event.Set();
        }

        private void ToastRun()
        {
            while (true)
            {
                if (_toastQueue.IsEmpty)
                {
                    if (CurrentToast != null)
                    {
                        CurrentToast = null;
                        OnDeactivate();
                    }

                    _event.Wait();
                }

                _event.Reset();

                ToastData toastData;
                if (_toastQueue.TryDequeue(out toastData))
                {
                    OnActivate();

                    CurrentToast = toastData;
                    Thread.Sleep(toastData.DisplayTime);
                }
            }
        }

        #endregion Methods
    }
}