using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.Core.Views.NowPlaying
{
    [Export(typeof(NowPlayingView))]
    public partial class NowPlayingView : UserControl
    {
        public NowPlayingView()
        {
            InitializeComponent();
        }

        [Import]
        public NowPlayingViewModel Model
        {
            get { return DataContext as NowPlayingViewModel; }
            set { DataContext = value; }
        }
    }
}
