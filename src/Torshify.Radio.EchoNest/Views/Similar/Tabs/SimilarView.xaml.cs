using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.Views.Similar.Tabs
{
    [Export(typeof(SimilarView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SimilarView : UserControl
    {
        #region Constructors

        public SimilarView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public SimilarViewModel Model
        {
            get { return DataContext as SimilarViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox selector = (ListBox)sender;
            Model.UpdateCommandBar(selector.SelectedItems.OfType<SimilarArtistModel>());
        }

        #endregion Methods
    }
}