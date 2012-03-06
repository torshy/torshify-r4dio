using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.Views.Hot
{
    [Export]
    public partial class HotArtistsView : UserControl
    {
        public HotArtistsView()
        {
            InitializeComponent();
        }

        [Import]
        public HotArtistsViewModel Model
        {
            get
            {
                return DataContext as HotArtistsViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
