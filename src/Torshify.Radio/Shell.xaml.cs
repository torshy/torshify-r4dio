using System.ComponentModel.Composition;
using Torshify.Radio.Framework.Controls;

namespace Torshify.Radio
{
    [Export(typeof(Shell))]
    public partial class Shell : MetroWindow
    {
        public Shell()
        {
            InitializeComponent();
        }
    }
}
