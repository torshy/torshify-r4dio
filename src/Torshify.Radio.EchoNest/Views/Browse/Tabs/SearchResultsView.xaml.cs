using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Browse.Tabs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SearchResultsView
    {
        #region Constructors

        public SearchResultsView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public SearchResultsViewModel Model
        {
            get { return DataContext as SearchResultsViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid) sender;
            Model.UpdateCommandBar(dataGrid.SelectedItems.OfType<Track>());
        }

        #endregion Methods
    }
}