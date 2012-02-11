using System.Windows;
using System.Windows.Controls;

namespace Torshify.Radio.Framework.Controls
{
    public class PanoramaItem : ContentControl
    {
        #region Fields

        public static readonly DependencyProperty HeaderOpacityProperty = 
            DependencyProperty.Register(
                "HeaderOpacity", 
                typeof(double), 
                typeof(PanoramaItem), 
                new PropertyMetadata(1.0));
        public static readonly DependencyProperty HeaderProperty = 
            DependencyProperty.Register(
                "Header", 
                typeof(object), 
                typeof(PanoramaItem), 
                new PropertyMetadata(null));
        public static readonly DependencyProperty HeaderTemplateProperty = 
            DependencyProperty.Register(
                "HeaderTemplate", 
                typeof(DataTemplate), 
                typeof(PanoramaItem), 
                new PropertyMetadata(null));
        public static readonly DependencyProperty OrientationProperty = 
            DependencyProperty.Register(
                "Orientation", 
                typeof(Orientation), 
                typeof(PanoramaItem), 
                new PropertyMetadata(Orientation.Horizontal));

        #endregion Fields

        #region Constructors

        static PanoramaItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PanoramaItem), new FrameworkPropertyMetadata(typeof(PanoramaItem)));
        }

        #endregion Constructors

        #region Properties

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public double HeaderOpacity
        {
            get { return (double)GetValue(HeaderOpacityProperty); }
            set { SetValue(HeaderOpacityProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion Properties
    }
}