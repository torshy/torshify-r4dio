using System.Collections.Generic;
using System.Threading;

namespace Torshify.Radio.Framework
{
    public interface ITrackStream
    {
        bool SupportsTrackSkipping
        {
            get;
        }

        string Description
        {
            get;
        }

        TrackStreamData Data
        {
            get;
        }

        IEnumerable<Track> Current { get; }

        void Dispose();

        bool MoveNext(CancellationToken token);

        void Reset();
    }
}