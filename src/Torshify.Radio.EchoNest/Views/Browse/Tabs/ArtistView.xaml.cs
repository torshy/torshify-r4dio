using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.EchoNest.Views.Browse.Tabs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class ArtistView : UserControl
    {
        public ArtistView()
        {
            InitializeComponent();
        }

        [Import]
        public ArtistViewModel Model
        {
            get
            {
                return DataContext as ArtistViewModel;
            }
            set
            {
                DataContext = value;
            }
        }  
    }
}
