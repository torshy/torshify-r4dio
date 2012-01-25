using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Torshify.Radio.Framework.Controls
{
    [ContentProperty("Content")]
    public class CircleButton : Button
    {
         static CircleButton()
         {
             DefaultStyleKeyProperty.OverrideMetadata(typeof(CircleButton), new FrameworkPropertyMetadata(typeof(CircleButton)));
         }
    }
}