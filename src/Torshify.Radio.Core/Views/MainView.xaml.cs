using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.Core.Views
{
    [Export(typeof(MainView))]
    public partial class MainView : UserControl
    {
        #region Constructors

        public MainView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public MainViewModel Model
        {
            get { return DataContext as MainViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}