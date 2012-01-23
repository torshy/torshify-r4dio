using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.Core.Views.Controls
{
    [Export(typeof(ControlsView))]
    public partial class ControlsView : UserControl
    {
        public ControlsView()
        {
            InitializeComponent();
        }
        
        [Import]
        public ControlsViewModel Model
        {
            get { return DataContext as ControlsViewModel; }
            set { DataContext = value; }
        }  
    }
}
