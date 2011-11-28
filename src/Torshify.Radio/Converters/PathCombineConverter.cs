using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Torshify.Radio.Converters
{
    public class PathCombineConverter : IValueConverter
    {
        public string Folder
        {
            get; 
            set;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return Path.Combine(Folder, System.Convert.ToString(value));
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}