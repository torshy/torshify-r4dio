using System.Windows;
using System.Windows.Controls;

namespace Torshify.Radio.Framework.Controls
{
    public class MetroProgressBar : ProgressBar
    {
         static MetroProgressBar()
         {
             DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroProgressBar), new FrameworkPropertyMetadata(typeof(MetroProgressBar)));
         }
    }
}