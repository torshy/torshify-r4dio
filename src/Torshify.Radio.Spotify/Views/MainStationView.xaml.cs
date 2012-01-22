using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.Spotify.Views
{
    [Export(typeof(MainStationView))]
    public partial class MainStationView : UserControl
    {
        public MainStationView()
        {
            InitializeComponent();
        }

        [Import]
        public MainStationViewModel Model
        {
            get { return DataContext as MainStationViewModel; }
            set { DataContext = value; }
        }
    }
}
