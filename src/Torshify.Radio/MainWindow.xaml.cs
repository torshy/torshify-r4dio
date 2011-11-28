using System.ComponentModel.Composition;
using Torshify.Radio.Controls;

namespace Torshify.Radio
{
    [Export(typeof(MainWindow))]
    public partial class MainWindow : MetroWindow
    {
        [ImportingConstructor]
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            Model = viewModel;
            Model.View = this;
        }

        public MainWindowViewModel Model
        {
            get { return DataContext as MainWindowViewModel; }
            set { DataContext = value; }
        }
    }
}
