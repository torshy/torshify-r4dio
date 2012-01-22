using System.ComponentModel.Composition;
using System.Windows.Controls;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Spotify.Views
{
    [Export(typeof(MainStationView))]
    public partial class MainStationView : UserControl, IRadioStation
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

        public void OnTuneIn()
        {
        }

        public void OnTuneAway()
        {
        }
    }
}
