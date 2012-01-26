using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
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
            Window window = new Window();
            window.Content = new TextBlock
                             {
                                 Text = message, 
                                 FontSize = 24,
                                 HorizontalAlignment = HorizontalAlignment.Center, 
                                 VerticalAlignment = VerticalAlignment.Center
                             };
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.NoResize;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Owner = Application.Current.MainWindow;
            window.Show();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(displayTimeMs);
            timer.Tick += (sender, args) =>
                          {
                              ((DispatcherTimer)sender).Stop();
                              window.Close();
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