using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Controls;

namespace Torshify.Radio.EchoNest.Views.Browse.Tabs
{
    public partial class ArtistViewSmall : UserControl
    {
        #region Constructors

        public ArtistViewSmall()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;

            ArtistViewModel viewModel = DataContext as ArtistViewModel;

            if (viewModel != null)
            {
                viewModel.UpdateCommandBar(dataGrid.SelectedItems.OfType<Track>());
            }

            if (e.AddedItems.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _albumsTreeView.Items.Count; i++)
            {
                var container = _albumsTreeView.ItemContainerGenerator.ContainerFromIndex(i);

                if (container != null)
                {
                    var parent = container.FindVisualDescendantByType<DataGrid>();

                    if (parent != e.Source)
                    {
                        if (Keyboard.Modifiers != ModifierKeys.Control)
                        {
                            parent.SelectedItem = null;
                        }
                    }
                }
            }
        }

        #endregion Methods
    }
}