using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Torshify.Radio.Converters
{
    public class UriToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string url = value.ToString();
                var coverArtSource = new BitmapImage();
                coverArtSource.BeginInit();
                coverArtSource.CacheOption = BitmapCacheOption.None;
                coverArtSource.UriSource = new Uri(url, UriKind.Absolute);
                coverArtSource.EndInit();

                if (coverArtSource.CanFreeze)
                {
                    coverArtSource.Freeze();
                }

                return coverArtSource;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}