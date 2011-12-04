using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.Spotify.Views.Configuration
{
    [Export(typeof(ConfigurationView))]
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }
    }
}
