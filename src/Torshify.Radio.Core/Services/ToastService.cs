using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Services
{
    [Export(typeof(IToastService))]
    public class ToastService : IToastService
    {
        private ConcurrentQueue<ToastContainer> _toasts;

        public ToastService()
        {
            _toasts = new ConcurrentQueue<ToastContainer>();
        }

        public void Show(string message, double displayTimeMs = 1000)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action<string, double>(Show), message, displayTimeMs);
                return;
            }

            Popup popup = new Popup();
            popup.Child = new TextBlock
                             {
                                 Text = message, 
                                 FontSize = 24,
                                 Foreground = Brushes.White,
                                 HorizontalAlignment = HorizontalAlignment.Center, 
                                 VerticalAlignment = VerticalAlignment.Center
                             };
            popup.IsOpen = true;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(displayTimeMs);
            timer.Tick += (sender, args) =>
                          {
                              ((DispatcherTimer)sender).Stop();
                              popup.IsOpen = false;
                          };
            timer.Start();
        }

        public void Show(UIElement element, double displayTimeMs = 1000)
        {
            _toasts.Enqueue(new ToastContainer { Item = element, Timeout = displayTimeMs });
        }

        private class ToastContainer
        {
            public object Item { get; set; }
            public double Timeout { get; set; }
        }
    }
}