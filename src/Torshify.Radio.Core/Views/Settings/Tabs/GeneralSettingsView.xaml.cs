using System.ComponentModel.Composition;
using System.Windows.Controls;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Views.Settings.Tabs
{
    public partial class GeneralSettingsView : UserControl, ISettingsPage
    {
        #region Constructors

        public GeneralSettingsView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public GeneralSettingsViewModel Model
        {
            get
            {
                return DataContext as GeneralSettingsViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion Properties

        #region Methods

        private void ButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Model.Save();
        }

        #endregion Methods
    }
}