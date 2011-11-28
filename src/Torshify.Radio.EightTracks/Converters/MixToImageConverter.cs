using System;
using System.Globalization;
using System.Windows.Data;
using EightTracks;

namespace Torshify.Radio.EightTracks.Converters
{
    public class MixToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mix = value as Mix;

            if (mix != null)
            {
                var imageUrl = mix.CoverUrls.Max200
                    ?? mix.CoverUrls.Max133w
                    ?? mix.CoverUrls.SQ133
                    ?? mix.CoverUrls.SQ100
                    ?? mix.CoverUrls.SQ56
                    ?? mix.CoverUrls.Original;

                if (imageUrl != null && imageUrl.StartsWith("/images/"))
                {
                    return "http://www.8tracks.com" + imageUrl;
                }

                return imageUrl;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}