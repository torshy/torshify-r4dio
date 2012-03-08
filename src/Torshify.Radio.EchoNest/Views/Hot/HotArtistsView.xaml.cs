using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.Views.Hot
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class HotArtistsView : UserControl
    {
        #region Constructors

        public HotArtistsView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public HotArtistsViewModel Model
        {
            get
            {
                return DataContext as HotArtistsViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion Properties
    }
}