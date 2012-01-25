using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.Core.Views.NowPlaying
{
    [Export(typeof(NowPlayingView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
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
