using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.EchoNest.Views.Browse.Tabs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = true)]
    public partial class AlbumView
    {
        public AlbumView()
        {
            InitializeComponent();
        }

        [Import]
        public AlbumViewModel Model
        {
            get
            {
                return DataContext as AlbumViewModel;
            }
            set
            {
                DataContext = value;
            }
        }  

    }
}
