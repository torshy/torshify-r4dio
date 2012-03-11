using System.Windows;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.Views.Hot
{
    public class AniScrollViewer : ScrollViewer
    {
        #region Fields

        public static DependencyProperty CurrentHorizontalOffsetProperty = DependencyProperty.Register("CurrentHorizontalOffsetOffset", typeof(double), typeof(AniScrollViewer), new PropertyMetadata(new PropertyChangedCallback(OnHorizontalChanged)));

        public static DependencyProperty CurrentVerticalOffsetProperty = DependencyProperty.Register("CurrentVerticalOffset", typeof(double), typeof(AniScrollViewer), new PropertyMetadata(new PropertyChangedCallback(OnVerticalChanged)));

        #endregion Fields

        #region Properties

        public double CurrentHorizontalOffset
        {
            get
            {
                return (double)this.GetValue(CurrentHorizontalOffsetProperty);
            }
            set
            {
                this.SetValue(CurrentHorizontalOffsetProperty, value);
            }
        }

        public double CurrentVerticalOffset
        {
            get
            {
                return (double)this.GetValue(CurrentVerticalOffsetProperty);
            }
            set
            {
                this.SetValue(CurrentVerticalOffsetProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        //When the DependencyProperty is changed change the vertical offset, thus 'animating' the scrollViewer
        private static void OnVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AniScrollViewer viewer = d as AniScrollViewer;
            viewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        //When the DependencyProperty is changed change the vertical offset, thus 'animating' the scrollViewer
        private static void OnHorizontalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AniScrollViewer viewer = d as AniScrollViewer;
            viewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        #endregion Methods
    }
}