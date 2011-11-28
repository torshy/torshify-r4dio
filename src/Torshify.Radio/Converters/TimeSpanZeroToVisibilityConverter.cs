using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Torshify.Radio.Framework.Common;

namespace Torshify.Radio.Converters
{
    public class TimeSpanZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan timeSpan = (TimeSpan) value;

            if (timeSpan == TimeSpan.Zero || DoubleUtilities.AreClose(timeSpan.TotalMilliseconds, 0.0))
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}