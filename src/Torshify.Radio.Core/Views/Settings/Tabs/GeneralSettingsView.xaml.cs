using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Windows.Input;
using Torshify.Radio.Core.Services;
using Torshify.Radio.Framework;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace Torshify.Radio.Core.Views.Settings.Tabs
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
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

        public void Load()
        {
            Model.Load();
        }

        public void Save()
        {
            Model.Save();
        }

        #endregion Methods
    }
}