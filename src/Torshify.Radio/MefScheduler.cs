using System.ComponentModel.Composition;

using Hardcodet.Scheduling;

namespace Torshify.Radio
{
    [Export(typeof(Scheduler))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefScheduler : Scheduler
    {
    }
}