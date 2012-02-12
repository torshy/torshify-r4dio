using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Torshify.Radio.Core.Views.NowPlaying.UI.Converters
{
    public class BrushOpacityConverter : IValueConverter
    {
        #region Constructors

        public BrushOpacityConverter()
        {
            Alpha = 127;
        }

        #endregion Constructors

        #region Properties

        public byte Alpha
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var solidColorBrush = value as SolidColorBrush;

            if (solidColorBrush != null)
            {
                var color = solidColorBrush.Color;
                return new SolidColorBrush(Color.FromArgb(Alpha, color.R, color.G, color.B));
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}