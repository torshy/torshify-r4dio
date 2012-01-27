using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface ITrackStream : IEnumerator<IEnumerable<Track>>
    {
        IRadioStation Station { get; }
    }
}