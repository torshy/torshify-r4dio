using System.ComponentModel.Composition;
using System.Windows.Controls;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Views.Settings.Tabs
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class NotGeneralSettingsView : UserControl, ISettingsPage
    {
        #region Constructors

        public NotGeneralSettingsView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public NotGeneralSettingsViewModel Model
        {
            get
            {
                return DataContext as NotGeneralSettingsViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion Properties

        #region Methods

        public void Apply()
        {
        }

        #endregion Methods
    }
}