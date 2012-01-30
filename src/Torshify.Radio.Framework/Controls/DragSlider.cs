using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Torshify.Radio.Framework.Controls
{
    public class DragSlider : Slider
    {
        #region Fields

        private Thumb _thumb = null;

        #endregion Fields

        #region Constructors

        static DragSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragSlider), new FrameworkPropertyMetadata(typeof(DragSlider)));
        }

        #endregion Constructors

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_thumb != null)
            {
                _thumb.MouseEnter -= thumb_MouseEnter;
            }

            var track = Template.FindName("PART_Track", this) as System.Windows.Controls.Primitives.Track;

            if (track != null)
            {
                _thumb = track.Thumb;

                if (_thumb != null)
                {
                    _thumb.MouseEnter += thumb_MouseEnter;
                }
            }
        }

        private void thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // the left button is pressed on mouse enter
                // so the thumb must have been moved under the mouse
                // in response to a click on the track.
                // Generate a MouseLeftButtonDown event.
                MouseButtonEventArgs args = new MouseButtonEventArgs(
                    e.MouseDevice, e.Timestamp, MouseButton.Left);
                    args.RoutedEvent = MouseLeftButtonDownEvent;

                ((Thumb)sender).RaiseEvent(args);
            }
        }

        #endregion Methods
    }
}