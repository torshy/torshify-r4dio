using System.Windows;
using System.Windows.Controls;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Views.Settings
{
    public class SettingsPageTemplateSelector : DataTemplateSelector
    {
        #region Properties

        public DataTemplate PageTemplate
        {
            get; set;
        }

        public DataTemplate SectionTemplate
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ISettingsPage)
            {
                return PageTemplate;
            }

            if (item is ISettingsSection)
            {
                return SectionTemplate;
            }

            return base.SelectTemplate(item, container);
        }

        #endregion Methods
    }
}