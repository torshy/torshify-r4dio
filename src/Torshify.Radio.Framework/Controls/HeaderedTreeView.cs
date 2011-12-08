using System.Windows;
using System.Windows.Controls;

namespace Torshify.Radio.Framework.Controls
{
    public class HeaderedTreeView : TreeView
    {
        #region Fields

        public static readonly DependencyProperty HeaderProperty = 
            DependencyProperty.Register("Header", typeof(object), typeof(HeaderedTreeView),
                new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty HeaderTemplateProperty = 
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HeaderedTreeView),
                new FrameworkPropertyMetadata((DataTemplate)null));
        public static readonly DependencyProperty HeaderTemplateSelectorProperty = 
            DependencyProperty.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(HeaderedTreeView),
                new FrameworkPropertyMetadata((DataTemplateSelector)null));

        #endregion Fields

        #region Constructors

        static HeaderedTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedTreeView), new FrameworkPropertyMetadata(typeof(HeaderedTreeView)));
        }

        #endregion Constructors

        #region Properties

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public DataTemplateSelector HeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
            set { SetValue(HeaderTemplateSelectorProperty, value); }
        }

        #endregion Properties
    }
}