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

        #endregion Methods
    }
}