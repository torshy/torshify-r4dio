using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Torshify.Radio.Framework.Behaviors
{
    /// <summary>
    /// Captures and eats MouseWheel events so that a nested ListBox does not
    /// prevent an outer scrollable control from scrolling.
    /// </summary>
    public sealed class IgnoreMouseWheelBehavior : Behavior<UIElement>
    {
        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += AssociatedObjectPreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObjectPreviewMouseWheel;
            base.OnDetaching();
        }

        private void AssociatedObjectPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            AssociatedObject.RaiseEvent(e2);
        }

        #endregion Methods
    }
}