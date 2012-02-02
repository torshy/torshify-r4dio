using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface ITrackStream : IEnumerator<IEnumerable<Track>>
    {
        bool SupportsTrackSkipping
        {
            get;
        }

        string Description
        {
            get;
        }
    }
}