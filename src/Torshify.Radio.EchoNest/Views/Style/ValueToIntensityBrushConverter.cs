using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Torshify.Radio.EchoNest.Views.Style
{
    public class ValueToIntensityBrushConverter : IValueConverter
    {
        #region Constructors

        public ValueToIntensityBrushConverter()
        {
            MostIntensiveColor = Colors.Red;
            LeastIntensiveColor = Colors.Black;
            Maximum = 1.0;
        }

        #endregion Constructors

        #region Properties

        public Color LeastIntensiveColor
        {
            get;
            set;
        }

        public Color MostIntensiveColor
        {
            get;
            set;
        }

        public double Maximum
        {
            get; 
            set;
        }

        #endregion Properties

        #region Methods

        public static float Lerp(float start, float end, float amount)
        {
            float difference = end - start;
            float adjusted = difference * amount;
            return start + adjusted;
        }

        public static Color Lerp(Color colour, Color to, float amount)
        {
            // start colours as lerp-able floats
            float sr = colour.R, sg = colour.G, sb = colour.B;

            // end colours as lerp-able floats
            float er = to.R, eg = to.G, eb = to.B;

            // lerp the colours to get the difference
            byte r = (byte)Lerp(sr, er, amount),
                 g = (byte)Lerp(sg, eg, amount),
                 b = (byte)Lerp(sb, eb, amount);

            // return the new colour
            return Color.FromRgb(r, g, b);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double hotttnesss = (System.Convert.ToDouble(value) / Maximum);

            HslColor c = new HslColor(MostIntensiveColor);
            c.Hue = hotttnesss * 255;

            Color color = (Color)c;
            return new SolidColorBrush(Lerp(LeastIntensiveColor, color, (float)hotttnesss));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}