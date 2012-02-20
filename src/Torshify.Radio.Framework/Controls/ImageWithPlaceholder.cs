using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Torshify.Radio.Framework.Controls
{
    public class ImageWithPlaceholder : Image
    {
        #region Fields

        public static readonly DependencyProperty ImageUriProperty = 
            DependencyProperty.Register(
                "ImageUri",
                typeof(Uri),
                typeof(ImageWithPlaceholder),
                new PropertyMetadata(null, OnImageUriChanged));

        private BitmapImage _loadedImage;

        #endregion Fields

        #region Properties

        public string LoadFailedImage
        {
            get;
            set;
        }

        public Uri ImageUri
        {
            get
            {
                return GetValue(ImageUriProperty) as Uri;
            }
            set
            {
                SetValue(ImageUriProperty, value);
            }
        }

        public string InitialImage
        {
            get;
            set;
        }

        private new ImageSource Source
        {
            get
            {
                return base.Source;
            }
            set
            {
                base.Source = value;
            }
        }

        #endregion Properties

        #region Methods

        private static void OnImageUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ImageWithPlaceholder) d).OnImageUriChanged(e);
        }

        private void OnImageUriChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                // Loading the specified image
                _loadedImage = new BitmapImage();
                _loadedImage.BeginInit();
                _loadedImage.CacheOption = BitmapCacheOption.OnDemand;
                _loadedImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                _loadedImage.DownloadCompleted += OnDownloadCompleted;
                _loadedImage.DownloadFailed += OnDownloadFailed;
                _loadedImage.UriSource = (Uri)e.NewValue;
                _loadedImage.EndInit();

                // The image may be cached, in which case we will not use the initial image
                if (!_loadedImage.IsDownloading)
                {
                    Source = _loadedImage;
                }
                else
                {
                    // Create InitialImage source if path is specified
                    if (!string.IsNullOrWhiteSpace(InitialImage))
                    {
                        BitmapImage initialImage = new BitmapImage();

                        // Load the initial bitmap from the local resource
                        initialImage.BeginInit();
                        initialImage.UriSource = new Uri(InitialImage, UriKind.RelativeOrAbsolute);
                        initialImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        initialImage.EndInit();

                        // Set the initial image as the image source
                        Source = initialImage;
                    }
                }
            }
        }

        private void OnDownloadFailed(object sender, ExceptionEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(LoadFailedImage))
            {
                BitmapImage failedImage = new BitmapImage();

                // Load the initial bitmap from the local resource
                failedImage.BeginInit();
                failedImage.UriSource = new Uri(LoadFailedImage, UriKind.RelativeOrAbsolute);
                failedImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                failedImage.EndInit();
                Source = failedImage;
            }
        }

        private void OnDownloadCompleted(object sender, EventArgs e)
        {
            Source = _loadedImage;
        }

        #endregion Methods
    }
}