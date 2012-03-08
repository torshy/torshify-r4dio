using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Torshify.Radio.Framework.Controls
{
    public class CompositionTargetRenderingListener : DispatcherObject, IDisposable
    {
        #region Fields

        private bool _disposed;
        private bool _isListening;

        #endregion Fields

        #region Events

        public event EventHandler IsListeningChanged;

        public event EventHandler Rendering;

        #endregion Events

        #region Properties

        public bool IsListening
        {
            get
            {
                return _isListening;
            }
            private set
            {
                if (value != _isListening)
                {
                    _isListening = value;
                    OnIsListeneningChanged(EventArgs.Empty);
                }
            }
        }

        public bool IsDisposed
        {
            get
            {
                VerifyAccess();
                return _disposed;
            }
        }

        #endregion Properties

        #region Methods

        public void StartListening()
        {
            RequireAccessAndNotDisposed();

            if (!_isListening)
            {
                IsListening = true;
                CompositionTarget.Rendering += CompositionTargetRendering;
            }
        }

        public void StopListening()
        {
            RequireAccessAndNotDisposed();

            if (_isListening)
            {
                IsListening = false;
                CompositionTarget.Rendering -= CompositionTargetRendering;
            }
        }

        public void WireParentLoadedUnloaded(FrameworkElement parent)
        {
            RequireAccessAndNotDisposed();

            parent.Loaded += delegate(object sender, RoutedEventArgs e)
            {
                this.StartListening();
            };

            parent.Unloaded += delegate(object sender, RoutedEventArgs e)
            {
                this.StopListening();
            };
        }

        public void Dispose()
        {
            RequireAccessAndNotDisposed();
            StopListening();

            Rendering
              .GetInvocationList()
              .ForEach(d => Rendering -= (EventHandler)d);

            _disposed = true;
        }

        protected virtual void OnRendering(EventArgs args)
        {
            RequireAccessAndNotDisposed();

            EventHandler handler = Rendering;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnIsListeneningChanged(EventArgs args)
        {
            var handler = IsListeningChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void RequireAccessAndNotDisposed()
        {
            VerifyAccess();
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            OnRendering(e);
        }

        #endregion Methods
    }
}