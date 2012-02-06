using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.Core.Views.Notifications
{
    [Export]
    public partial class ToastServiceView : UserControl
    {
        public ToastServiceView()
        {
            InitializeComponent();
        }

        [Import]
        public ToastServiceViewModel Model
        {
            get { return DataContext as ToastServiceViewModel; }
            set { DataContext = value; }
        }  
    }
}
