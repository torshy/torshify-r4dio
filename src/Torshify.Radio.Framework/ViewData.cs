using System;
using System.Windows;

namespace Torshify.Radio.Framework
{
    public class ViewData : DependencyObject
    {
        #region Fields

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ViewData),
                new FrameworkPropertyMetadata("Header"));
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ViewData),
                new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof(Lazy<UIElement>), typeof(ViewData),
                                        new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(ViewData),
                new FrameworkPropertyMetadata(Visibility.Visible));
        
        #endregion Fields

        #region Properties

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public Lazy<UIElement> View
        {
            get { return (Lazy<UIElement>)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        #endregion Properties
    }
}