using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.Browse
{
    [Export(typeof(ArtistBrowseView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ArtistBrowseView : UserControl
    {
        public ArtistBrowseView()
        {
            InitializeComponent();
        }

        #region Properties

        [Import]
        public ArtistBrowseViewModel Model
        {
            get { return DataContext as ArtistBrowseViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}
