using System.Windows.Input;

namespace Torshify.Radio.Framework.Input
{
    public class ExtendedMouseGesture : MouseGesture
    {
        #region Fields

        private readonly MouseButton _mouseButton;

        #endregion Fields

        #region Constructors

        public ExtendedMouseGesture(MouseButton mouseButton)
        {
            _mouseButton = mouseButton;
        }

        #endregion Constructors

        #region Methods

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            var device = inputEventArgs.Device as MouseDevice;

            if (device != null)
            {
                switch (_mouseButton)
                {
                    case MouseButton.XButton1:
                        if (device.XButton1 == MouseButtonState.Pressed) return true;
                        break;
                    case MouseButton.XButton2:
                        if (device.XButton2 == MouseButtonState.Pressed) return true;
                        break;
                }
            }

            return false;
        }

        #endregion Methods
    }
}