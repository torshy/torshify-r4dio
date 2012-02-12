using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.Framework.Input;

namespace Torshify.Radio.Core.Views.NowPlaying
{
    [Export(typeof(NowPlayingView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class NowPlayingView : UserControl
    {
        #region Constructors

        public NowPlayingView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public NowPlayingViewModel Model
        {
            get { return DataContext as NowPlayingViewModel; }
            set
            {
                DataContext = value;
                InputBindings.Add(new ExtendedMouseBinding
                {
                    Command = value.NavigateBackCommand,
                    Gesture = new ExtendedMouseGesture(MouseButton.XButton1)
                });
            }
        }

        #endregion Properties

        #region Methods

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            // Prevent the NavigateForward command to execute in the background while displaying the NowPlaying view.
            if (e.ChangedButton == MouseButton.XButton2)
            {
                e.Handled = true;
                return;
            }

            base.OnPreviewMouseDown(e);
        }

        #endregion Methods
    }
}