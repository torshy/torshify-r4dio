using System.ComponentModel.Composition;
using System.Windows.Controls;

using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.EchoNest.Views.Favorites.Tabs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = true)]
    public partial class FavoritesView : UserControl
    {
        #region Constructors

        public FavoritesView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public FavoritesViewModel Model
        {
            get
            {
                return DataContext as FavoritesViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion Properties
    }
}