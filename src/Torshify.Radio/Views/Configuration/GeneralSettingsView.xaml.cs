using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.Views.Configuration
{
    [Export(typeof(GeneralSettingsView))]
    public partial class GeneralSettingsView : UserControl
    {
        public GeneralSettingsView()
        {
            InitializeComponent();
        }
    }
}
