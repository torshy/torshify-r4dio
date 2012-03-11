using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;

namespace Torshify.Radio.Framework.Events
{
    public class ApplicationArgumentsEvent : CompositePresentationEvent<IEnumerable<string>>
    {
         
    }
}