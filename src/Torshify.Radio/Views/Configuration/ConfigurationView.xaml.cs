using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.Views.Configuration
{
    [Export(typeof(ConfigurationView))]
    public partial class ConfigurationView : UserControl
    {
        #region Constructors

        public ConfigurationView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public ConfigurationViewModel Model
        {
            get { return DataContext as ConfigurationViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}