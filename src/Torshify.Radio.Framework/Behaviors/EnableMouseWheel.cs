using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Torshify.Radio.Framework.Behaviors
{
    [System.ComponentModel.Description("Target : Trigger. Responds to mousewheel event of attached object when focused. Manipulates slider values based on slider properties.")]
    public class EnableMouseWheel : TargetedTriggerAction<Slider>
    {
        #region Methods

        //this method is required for the base class
        protected override void Invoke(object parameter)
        {
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            //add event handler to mouse wheel event
            ((FrameworkElement)AssociatedObject).MouseWheel += SliderMouseScrollMouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            //remove the event handler
            ((FrameworkElement)AssociatedObject).MouseWheel -= SliderMouseScrollMouseWheel;
        }

        //this method runs on Mouse Wheel event
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

                Target.Value = Target.Value + Target.SmallChange;
            }
            else //same as above just backwards
            {
                //using small change of Target(slider) property.
                //this allows how much value can change from the designer
                //
                //alternatively you can actually use the delta value instead
                //of forcing small change increments.

                Target.Value = Target.Value - Target.SmallChange;
            }
        }

        #endregion Methods
    }
}