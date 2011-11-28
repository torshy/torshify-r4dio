using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism;
using System.Linq;
using Torshify.Radio.Controls;

namespace Torshify.Radio
{
    public partial class RadioNowPlayingViewMedium : UserControl
    {
        #region Constructors

        public RadioNowPlayingViewMedium()
        {
            InitializeComponent();

            Loaded += OnViewLoaded;
            Unloaded += OnViewUnloaded;
        }

        #endregion Constructors

        #region Methods

        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            var model = DataContext as RadioNowPlayingViewModel;
            
            Application.Current.MainWindow.Resources.Add(SystemColors.HighlightTextBrushKey, Brushes.White);
            Application.Current.MainWindow.Resources.Add(SystemColors.DesktopBrushKey, new SolidColorBrush(Color.FromArgb(100, 0, 192, 255)));

            if (model != null)
            {
                var regionManager = ServiceLocator.Current.TryResolve<IRegionManager>();
                var region = regionManager.Regions["BackgroundRegion"];
                var kenBurnsBackground = region.Views.OfType<KenBurnsPhotoFrame>().FirstOrDefault();

                if (kenBurnsBackground == null)
                {
                    kenBurnsBackground = new KenBurnsPhotoFrame();
                    region.Add(kenBurnsBackground);
                    region.Add(new ColorOverlayFrame());
                }

                BackdropService backdropService = new BackdropService();
                backdropService.CacheLocation = Path.Combine(Environment.CurrentDirectory, "Cache");
                backdropService.GetBackdrop(model.CurrentTrack.Artist, OnBackdropFound);
            }
        }

        private void OnBackdropFound(string backdrop)
        {
            var coverArtSource = new BitmapImage();
            coverArtSource.BeginInit();
            coverArtSource.CacheOption = BitmapCacheOption.None;
            coverArtSource.UriSource = new Uri(backdrop, UriKind.Absolute);
            coverArtSource.EndInit();

            if (coverArtSource.CanFreeze)
            {
                coverArtSource.Freeze();
            }

            Dispatcher.BeginInvoke(new Action<ImageSource>(OnShowBackdrop), coverArtSource);
        }

        private void OnShowBackdrop(ImageSource imageSource)
        {
            var regionManager = ServiceLocator.Current.TryResolve<IRegionManager>();
            var region = regionManager.Regions["BackgroundRegion"];
            var kenBurnsBackground = region.Views.OfType<KenBurnsPhotoFrame>().FirstOrDefault();

            if (kenBurnsBackground == null)
            {
                kenBurnsBackground = new KenBurnsPhotoFrame();
                region.Add(kenBurnsBackground);
                region.Add(new ColorOverlayFrame());
            }

            kenBurnsBackground.SetImageSource(imageSource);
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Resources.Clear();

            var regionManager = ServiceLocator.Current.TryResolve<IRegionManager>();
            var region = regionManager.Regions["BackgroundRegion"];
            var kenBurnsBackground = region.Views.OfType<KenBurnsPhotoFrame>().FirstOrDefault();

            if (kenBurnsBackground != null)
            {
                region.Remove(kenBurnsBackground);
            }

            var colorOverlayBackgrond = region.Views.OfType<ColorOverlayFrame>().FirstOrDefault();

            if (colorOverlayBackgrond != null)
            {
                region.Remove(colorOverlayBackgrond);
            }
        }

        #endregion Methods
    }
}