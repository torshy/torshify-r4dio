using System;
using System.Globalization;
using System.Windows.Data;

namespace Torshify.Radio.Framework.Converters
{
    public class StringPairToTupleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2 && values[0] != null && values[1] != null)
            {
                return Tuple.Create(values[0].ToString(), values[1].ToString());
            }

            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}