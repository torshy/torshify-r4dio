using System;
using System.Windows.Data;

namespace Torshify.Radio.Framework.Converters
{
    public class CountToFontSizeConverter : IValueConverter
    {
        #region Constructors

        public CountToFontSizeConverter()
        {
            MinimumFontSize = 10;
            MaximumFontSize = 38;
            Increment = 3;
        }

        #endregion Constructors

        #region Properties

        public int Increment
        {
            get; set;
        }

        public int MaximumFontSize
        {
            get; set;
        }

        public int MinimumFontSize
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int count = (int)value;

            return ((MinimumFontSize + count + Increment) < MaximumFontSize) ? (MinimumFontSize + count + Increment) : MaximumFontSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion Methods
    }
}