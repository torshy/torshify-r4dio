using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Practices.Prism.Regions;
using Torshify.Radio.Framework;
using Raven.Client.Linq.Indexing;
using Torshify.Radio.Framework.Controls;
using System.Linq;

namespace Torshify.Radio.Core.Views.Settings
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class SettingsView : UserControl
    {
        #region Constructors

        public SettingsView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public SettingsViewModel Model
        {
            get
            {
                return DataContext as SettingsViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion Properties

        #region Methods

        private void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ISettingsPage page = e.NewValue as ISettingsPage;
            ISettingsSection section = e.NewValue as ISettingsSection;

            if (page == null && section != null)
            {
                foreach (var settingsPage in Model.SettingPages)
                {
                    if (settingsPage.Sections.Contains(section))
                    {
                        page = settingsPage;
                        break;
                    }
                }
            }

            Model.CurrentPage = page;

            if (section != null)
            {
                ScrollSectionIntoView(section);
            }
        }

        private void ScrollSectionIntoView(ISettingsSection section)
        {
            // TODO
        }

        #endregion Methods
    }
}