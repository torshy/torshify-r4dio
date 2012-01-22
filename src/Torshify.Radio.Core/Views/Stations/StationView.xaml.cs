using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.Core.Views.Stations
{
    [Export(typeof(StationView))]
    public partial class StationView : UserControl
    {
        public StationView()
        {
            InitializeComponent();
        }

        [Import]
        public StationViewModel Model
        {
            get { return DataContext as StationViewModel; }
            set { DataContext = value; }
        }
    }
}
