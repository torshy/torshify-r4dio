using System.ComponentModel.Composition;
using System.Windows.Controls;

using Microsoft.Practices.Prism.Regions;

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
    }
}