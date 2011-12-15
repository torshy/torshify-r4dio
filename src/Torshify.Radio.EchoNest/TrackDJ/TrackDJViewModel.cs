using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest.TrackDJ
{
    [Export(typeof(TrackDJViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TrackDJViewModel : NotificationObject
    {

    }
}