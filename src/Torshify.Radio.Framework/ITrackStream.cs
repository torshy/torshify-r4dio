using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface ITrackStream : IEnumerator<Track>
    {
        IRadioStation Station { get; }
    }
}