using System.Windows;
using System.Windows.Controls;

namespace Torshify.Radio.Framework.Controls
{
    [TemplatePart(Name = "PART_TopHeaderScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_LeftHeaderScrollViewer", Type = typeof(ScrollViewer))]
    public class HeaderedScrollViewer : ScrollViewer
    {
        #region Fields

        public static readonly DependencyProperty LeftHeaderProperty = 
            DependencyProperty.Register("LeftHeader", typeof(object), typeof(HeaderedScrollViewer), new UIPropertyMetadata(LeftHeader_PropertyChanged));
        public static readonly DependencyProperty LeftHeaderTemplateProperty = 
            DependencyProperty.Register("LeftHeaderTemplate", typeof(DataTemplate), typeof(HeaderedScrollViewer), new UIPropertyMetadata());
        public static readonly DependencyProperty TopHeaderProperty = 
            DependencyProperty.Register("TopHeader", typeof(object), typeof(HeaderedScrollViewer), new UIPropertyMetadata(TopHeader_PropertyChanged));
        public static readonly DependencyProperty TopHeaderTemplateProperty = 
            DependencyProperty.Register("TopHeaderTemplate", typeof(DataTemplate), typeof(HeaderedScrollViewer), new UIPropertyMetadata());

        private ScrollViewer leftHeaderScrollViewer;
        private ScrollViewer topHeaderScrollViewer;

        #endregion Fields

        #region Constructors

        static HeaderedScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedScrollViewer), new FrameworkPropertyMetadata(typeof(HeaderedScrollViewer)));
        }

        #endregion Constructors

        #region Properties

        public object LeftHeader
        {
            get { return (object)GetValue(LeftHeaderProperty); }
            set { SetValue(LeftHeaderProperty, value); }
        }

        public DataTemplate LeftHeaderTemplate
        {
            get { return (DataTemplate)GetValue(LeftHeaderTemplateProperty); }
            set { SetValue(LeftHeaderTemplateProperty, value); }
        }

        public object TopHeader
        {
            get { return (object)GetValue(TopHeaderProperty); }
            set { SetValue(TopHeaderProperty, value); }
        }

        public DataTemplate TopHeaderTemplate
        {
            get { return (DataTemplate)GetValue(TopHeaderTemplateProperty); }
            set { SetValue(TopHeaderTemplateProperty, value); }
        }

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                if (TopHeader != null || LeftHeader != null)
                {

                    System.Collections.ArrayList children = new System.Collections.ArrayList();
                    System.Collections.IEnumerator baseEnumerator = base.LogicalChildren;
                    while (baseEnumerator.MoveNext())
                    {
                        children.Add(baseEnumerator.Current);
                    }
                    if (TopHeader != null)
                        children.Add(TopHeader);
                    if (LeftHeader != null)
                        children.Add(LeftHeader);
                    return children.GetEnumerator();
                }
                return base.LogicalChildren;
            }
        }

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            topHeaderScrollViewer = base.GetTemplateChild("PART_TopHeaderScrollViewer") as ScrollViewer;
            if (topHeaderScrollViewer != null)
                topHeaderScrollViewer.ScrollChanged += topHeaderScrollViewer_ScrollChanged;
            leftHeaderScrollViewer = base.GetTemplateChild("PART_LeftHeaderScrollViewer") as ScrollViewer;
            if (leftHeaderScrollViewer != null)
                leftHeaderScrollViewer.ScrollChanged += leftHeaderScrollViewer_ScrollChanged;
        }

        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            base.OnScrollChanged(e);
            if (topHeaderScrollViewer != null)
                topHeaderScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            if (leftHeaderScrollViewer != null)
                leftHeaderScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private static void LeftHeader_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedScrollViewer instance = (HeaderedScrollViewer)d;
            if (e.OldValue != null)
                instance.RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                instance.AddLogicalChild(e.NewValue);
        }

        private static void TopHeader_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedScrollViewer instance = (HeaderedScrollViewer)d;
            if (e.OldValue != null)
                instance.RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                instance.AddLogicalChild(e.NewValue);
        }

        private void leftHeaderScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource == leftHeaderScrollViewer)
                ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void topHeaderScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource == topHeaderScrollViewer)
                ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        #endregion Methods
    }
}