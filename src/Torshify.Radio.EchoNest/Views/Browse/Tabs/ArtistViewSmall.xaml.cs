using System.Collections.Generic;
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
            List<Track> selectedTracks = new List<Track>();
            for (int i = 0; i < _albumsTreeView.Items.Count; i++)
            {
                var container = _albumsTreeView.ItemContainerGenerator.ContainerFromIndex(i);

                if (container != null)
                {
                    var parent = container.FindVisualDescendantByType<DataGrid>();

                    if (parent == null)
                    {
                        continue;
                    }

                    if (parent != e.Source)
                    {
                        if (Keyboard.Modifiers != ModifierKeys.Control)
                        {
                            parent.SelectedItem = null;
                        }
                    }

                    selectedTracks.AddRange(parent.SelectedItems.OfType<Track>());
                }
            }

            ArtistViewModel viewModel = DataContext as ArtistViewModel;
            if (viewModel != null)
            {
                viewModel.UpdateCommandBar(selectedTracks);
            }
        }

        private void DataGridKeyDown(object sender, KeyEventArgs e)
        {
            var dataGrid = ((DataGrid)sender);

            if (e.Key == Key.Up)
            {
                if (dataGrid.SelectedIndex == 0)
                {
                    var parent = dataGrid.FindParent<TreeViewItem>();

                    if (parent != null)
                    {
                        int currentIndex = _albumsTreeView.ItemContainerGenerator.IndexFromContainer(parent) - 1;

                        if (currentIndex >= 0)
                        {
                            var container = _albumsTreeView.ItemContainerGenerator.ContainerFromIndex(currentIndex) as TreeViewItem;
                            dataGrid.SelectedItem = null;
                            var previousDatagrid = container.FindVisualDescendantByType<DataGrid>();

                            if (previousDatagrid != null)
                            {
                                previousDatagrid.SelectedIndex = previousDatagrid.Items.Count - 1;
                            }

                            Keyboard.Focus(container);
                        }
                    }
                }
            }
            else if (e.Key == Key.Down)
            {
                if (dataGrid.SelectedIndex == dataGrid.Items.Count - 1)
                {
                    var parent = dataGrid.FindParent<TreeViewItem>();

                    if (parent != null)
                    {
                        int currentIndex = _albumsTreeView.ItemContainerGenerator.IndexFromContainer(parent) + 1;

                        if (currentIndex < _albumsTreeView.Items.Count)
                        {
                            var container = _albumsTreeView.ItemContainerGenerator.ContainerFromIndex(currentIndex) as TreeViewItem;
                            dataGrid.SelectedItem = null;
                            var nextDatagrid = container.FindVisualDescendantByType<DataGrid>();

                            if (nextDatagrid != null)
                            {
                                nextDatagrid.SelectedIndex = 0;
                            }

                            Keyboard.Focus(container);
                        }
                    }
                }
            }
        }

        #endregion Methods
    }
}