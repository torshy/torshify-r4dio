using System.Windows;
using System.Windows.Controls;

namespace Torshify.Radio.Framework.Controls
{
    public class WatermarkAutoCompleteBox : AutoCompleteBox
    {
        #region Fields

        public static readonly DependencyProperty WatermarkProperty = 
            DependencyProperty.Register("Watermark", typeof(object), typeof(WatermarkAutoCompleteBox),
                                        new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty WatermarkTemplateProperty = 
            DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(WatermarkAutoCompleteBox),
                                        new FrameworkPropertyMetadata((DataTemplate)null));

        #endregion Fields

        #region Constructors

        static WatermarkAutoCompleteBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(WatermarkAutoCompleteBox),
                new FrameworkPropertyMetadata(typeof(WatermarkAutoCompleteBox)));
        }

        #endregion Constructors

        #region Properties

        public object Watermark
        {
            get { return GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        #endregion Properties
    }
}