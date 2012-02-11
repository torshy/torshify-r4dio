using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;

namespace Torshify.Radio.Framework.Input
{
    public class ExtendedMouseBinding : MouseBinding
    {
        #region Properties

        [ValueSerializer(typeof(MouseGestureValueSerializer)),
        TypeConverter(typeof(ExtendedMouseGestureConverter))]
        public override InputGesture Gesture
        {
            get
            {
                return base.Gesture;
            }
            set
            {
                base.Gesture = value;
            }
        }

        #endregion Properties
    }
}