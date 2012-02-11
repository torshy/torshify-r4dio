using System.ComponentModel;
using System.Windows.Input;

namespace Torshify.Radio.Framework.Input
{
    public class ExtendedMouseGestureConverter : MouseGestureConverter
    {
        #region Methods

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object source)
        {
            switch (source.ToString())
            {
                case "XButton1":
                    return new ExtendedMouseGesture(MouseButton.XButton1);
                case "XButton2":
                    return new ExtendedMouseGesture(MouseButton.XButton2);
            }
            return base.ConvertFrom(context, culture, source);
        }

        #endregion Methods
    }
}