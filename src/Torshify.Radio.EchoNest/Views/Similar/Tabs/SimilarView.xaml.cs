using System.ComponentModel.Composition;
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
    }
}