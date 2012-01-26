using System;
using System.Globalization;
using System.Windows.Data;

namespace Torshify.Radio.EightTracks.Converters
{
    public class DateDifferenceConverter : IValueConverter, IMultiValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                var dateTime = (DateTime)value;

                return GetDifferenceDate(DateTime.Now, dateTime);
            }

            return value;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2)
            {
                return GetDifferenceDate((DateTime)values[0], (DateTime)values[1]);
            }

            return values;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        static string GetDifferenceDate(DateTime date1, DateTime date2)
        {
            if (DateTime.Compare(date1, date2) >= 0)
            {
                TimeSpan ts = date1.Subtract(date2);

                if (ts.Days > 0)
                {
                    return string.Format("{0} days ago", ts.Days);
                }

                if (ts.Hours > 0)
                {
                    if (ts.Hours > 1)
                    {
                        return string.Format("{0} hours ago", ts.Hours);
                    }

                    return string.Format("{0} hour ago", ts.Hours);
                }

                if (ts.Minutes > 0)
                {
                    if (ts.Minutes > 1)
                    {
                        return string.Format("{0} minutes ago", ts.Minutes);
                    }

                    return string.Format("{0} minute ago", ts.Minutes);
                }

                return string.Format("{0} days, {1} hours, {2} minutes, {3} seconds", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            }

            return "Not valid";
        }

        #endregion Methods
    }
}