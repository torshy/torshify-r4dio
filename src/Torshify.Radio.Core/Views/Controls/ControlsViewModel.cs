using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Core.Views.Controls
{
    [Export(typeof(ControlsViewModel))]
    public class ControlsViewModel : NotificationObject
    {
         
    }
}