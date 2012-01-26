using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Torshify.Radio.EightTracks.Converters
{
    public class ListContainsItemToBoolConverter : IMultiValueConverter
    {
        #region Methods

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2)
            {
                var list = (IEnumerable<string>)values[0];
                var item = values[1].ToString().Trim();

                return list.Contains(item);
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[2] {Binding.DoNothing, Binding.DoNothing};
        }

        #endregion Methods
    }
}