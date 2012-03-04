using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Torshify.Radio.Core.Views.Controls
{
    public class IsLessOrEqualToConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                return ((double)value) <= System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    public partial class ControlsPart : UserControl
    {
        public ControlsPart()
        {
            InitializeComponent();
        }
    }
}
