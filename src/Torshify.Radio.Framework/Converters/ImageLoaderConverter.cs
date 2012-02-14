using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Torshify.Radio.Framework.Converters
{
    public class ImageLoaderConverter : IValueConverter
    {
        public int? DecodePixelWidth { get; set; }
        public int? DecodePixelHeight { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
            {
                return DependencyProperty.UnsetValue;
            }

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            image.UriSource = new Uri(value.ToString(), UriKind.RelativeOrAbsolute);

            if (DecodePixelHeight.HasValue)
            {
                image.DecodePixelHeight = DecodePixelHeight.GetValueOrDefault();
            }

            if (DecodePixelWidth.HasValue)
            {
                image.DecodePixelWidth = DecodePixelWidth.GetValueOrDefault();
            }

            image.EndInit();

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}