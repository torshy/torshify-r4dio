using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.Core.Views.Stations
{
    [Export(typeof(StationsView))]
    public partial class StationsView : UserControl
    {
        public StationsView()
        {
            InitializeComponent();
        }
    }
}
