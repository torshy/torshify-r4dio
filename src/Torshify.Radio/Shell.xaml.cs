using System.ComponentModel.Composition;
using System.Windows;

namespace Torshify.Radio
{
    [Export(typeof(Shell))]
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();
        }
    }
}
