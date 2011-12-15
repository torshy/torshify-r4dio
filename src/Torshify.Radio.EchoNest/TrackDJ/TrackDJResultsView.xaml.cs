using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.TrackDJ
{
    [Export(typeof(TrackDJResultsView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TrackDJResultsView : UserControl
    {
        #region Constructors

        public TrackDJResultsView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public TrackDJResultsViewModel Model
        {
            get
            {
                return DataContext as TrackDJResultsViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion Properties
    }
}