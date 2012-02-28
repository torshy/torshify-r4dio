using System.Windows;
using System.Windows.Controls.Primitives;

namespace Torshify.Radio.Framework.Controls
{
    public class ToggleContentButton : ToggleButton
    {
        #region Fields

        public static readonly DependencyProperty CheckedContentProperty = 
            DependencyProperty.Register("CheckedContent", typeof(object), typeof(ToggleContentButton),
                new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty NonCheckedContentProperty = 
            DependencyProperty.Register("NonCheckedContent", typeof(object), typeof(ToggleContentButton),
                new FrameworkPropertyMetadata((object)null));

        #endregion Fields

        #region Constructors

        static ToggleContentButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleContentButton), new FrameworkPropertyMetadata(typeof(ToggleContentButton)));
        }

        #endregion Constructors

        #region Properties

        public object NonCheckedContent
        {
            get
            {
                return (object)GetValue(NonCheckedContentProperty);
            }
            set
            {
                SetValue(NonCheckedContentProperty, value);
            }
        }

        public object CheckedContent
        {
            get
            {
                return (object)GetValue(CheckedContentProperty);
            }
            set
            {
                SetValue(CheckedContentProperty, value);
            }
        }

        #endregion Properties
    }
}