using System;
using System.Globalization;
using System.Windows.Data;
using EightTracks;

namespace Torshify.Radio.EightTracks.Converters
{
    public class MixToTrackStreamDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mix = value as Mix;

            if (mix != null)
            {
                return new EightTracksMixTrackStreamData
                {
                    Name = mix.Name,
                    Image = (string)new MixToImageConverter().Convert(mix, null, null, null),
                    Description = mix.TagListCache,
                    MixId = mix.ID,
                    Source = "8tracks"
                };
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}