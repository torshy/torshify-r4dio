using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Torshify.Radio.Framework.Controls
{
    public class MetroButton : Button
    {
        #region Fields

        public static readonly DependencyProperty MetroImageSourceProperty = 
            DependencyProperty.Register(
                "MetroImageSource", 
                typeof(Visual), 
                typeof(MetroButton), 
                new PropertyMetadata(default(Visual)));

        #endregion Fields

        #region Constructors

        public MetroButton()
        {
            DefaultStyleKey = typeof(MetroButton);
        }

        #endregion Constructors

        #region Properties

        public Visual MetroImageSource
        {
            get
            {
                return (Visual)GetValue(MetroImageSourceProperty);
            }
            set
            {
                SetValue(MetroImageSourceProperty, value);
            }
        }

        #endregion Properties
    }
}