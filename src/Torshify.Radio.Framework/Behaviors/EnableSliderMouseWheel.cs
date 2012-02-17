using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Torshify.Radio.Framework.Behaviors
{
    public class EnableSliderMouseWheel : Behavior<Slider>
    {
        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();

            //attach event handler on mouse wheel event
            AssociatedObject.MouseWheel += SliderMouseScrollMouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            //remove event handler on mouse wheel event
            AssociatedObject.MouseWheel -= SliderMouseScrollMouseWheel;
        }

        private void SliderMouseScrollMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Delta = how much the wheel was scrolled. + for up - for down

            //check to see if Delta is positive.
            if (e.Delta > 0)
            {
                //using small change of Target(slider) property.
                //this allows how much value can change from the designer
                //
                //alternatively you can actually use the delta value instead
                //of forcing small change increments.

                AssociatedObject.Value = AssociatedObject.Value + AssociatedObject.SmallChange;
            }
            else //same as above just backwards
            {
                //using small change of Target(slider) property.
                //this allows how much value can change from the designer
                //
                //alternatively you can actually use the delta value instead
                //of forcing small change increments.

                AssociatedObject.Value = AssociatedObject.Value - AssociatedObject.SmallChange;
            }
        }

        #endregion Methods
    }
}