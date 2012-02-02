using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(FavoritesView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class FavoritesView : UserControl
    {
        public FavoritesView()
        {
            InitializeComponent();
        }

        [Import]
        public FavoritesViewModel Model
        {
            get { return DataContext as FavoritesViewModel; }
            set { DataContext = value; }
        }
    }
}
