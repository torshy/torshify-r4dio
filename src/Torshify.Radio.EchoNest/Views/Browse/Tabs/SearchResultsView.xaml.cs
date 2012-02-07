using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.Views.Browse.Tabs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SearchResultsView : UserControl
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
    }
}