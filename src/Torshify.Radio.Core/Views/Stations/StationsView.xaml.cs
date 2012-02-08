using System.ComponentModel.Composition;

namespace Torshify.Radio.Core.Views.Stations
{
    [Export(typeof(StationsView))]
    public partial class StationsView
    {
        public StationsView()
        {
            InitializeComponent();
        }

        [Import]
        public StationsViewModel Model
        {
            get { return DataContext as StationsViewModel; }
            set { DataContext = value; }
        }
    }
}
