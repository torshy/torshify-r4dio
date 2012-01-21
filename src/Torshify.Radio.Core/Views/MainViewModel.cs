using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Core.Views
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : NotificationObject
    {
         
    }
}