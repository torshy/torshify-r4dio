using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.Core.Views.Settings.General
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class GeneralSettingsView : UserControl
    {
        #region Constructors

        public GeneralSettingsView()
        {
            InitializeComponent();
        }

        #endregion Constructors


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


    }
}