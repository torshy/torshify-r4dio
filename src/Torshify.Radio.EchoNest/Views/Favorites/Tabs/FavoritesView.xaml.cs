using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Controls;

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

        #region Methods

        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox selector = (ListBox)sender;
            Model.UpdateCommandBar(selector.SelectedItems.OfType<Favorite>());
        }

        private void ListBoxReorderRequested(object sender, ReorderEventArgs e)
        {
            var reorderListBox = (ReorderListBox)e.OriginalSource;
            var dragginItem = (Favorite)reorderListBox.ItemContainerGenerator.ItemFromContainer(e.ItemContainer);
            var toItem = (Favorite)reorderListBox.ItemContainerGenerator.ItemFromContainer(e.ToContainer);

            Model.MoveItem(dragginItem, toItem);
        }

        #endregion Methods
    }
}