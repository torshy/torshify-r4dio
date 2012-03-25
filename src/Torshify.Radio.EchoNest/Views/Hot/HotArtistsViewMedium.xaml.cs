using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Torshify.Radio.EchoNest.Views.Hot
{
    public partial class HotArtistsViewMedium : UserControl
    {
        #region Constructors

        public HotArtistsViewMedium()
        {
            InitializeComponent();
            //Scroller.MouseMove += ScrollerOnMouseMove;
            Scroller.PreviewMouseWheel += ScrollerOnMouseWheel;
        }

        #endregion Constructors

        #region Methods

        private void ScrollerOnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ScrollToPosition(Scroller.CurrentHorizontalOffset - (Scroller.ViewportWidth / 2));
            }
            else if (e.Delta < 0)
            {
                ScrollToPosition(Scroller.CurrentHorizontalOffset + (Scroller.ViewportWidth / 2));
            }
        }

        private void ScrollerOnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(Scroller);
            var percent = position.X / Scroller.ActualWidth;

            if (percent > 0.9)
            {
                ScrollToPosition(Scroller.CurrentHorizontalOffset + (Scroller.ViewportWidth / 2));
            }
            else if (percent < 0.1)
            {
                ScrollToPosition(Scroller.CurrentHorizontalOffset - (Scroller.ViewportWidth / 2));
            }
        }

        private void ScrollToPosition(double x)
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = Scroller.HorizontalOffset;
            anim.To = x;
            anim.DecelerationRatio = .2;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            Storyboard sb = new Storyboard();
            sb.Children.Add(anim);
            Storyboard.SetTarget(anim, Scroller);
            Storyboard.SetTargetProperty(anim, new PropertyPath(AniScrollViewer.CurrentHorizontalOffsetProperty));
            BeginStoryboard(sb, HandoffBehavior.SnapshotAndReplace);
        }

        #endregion Methods
    }
}