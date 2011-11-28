using System.Collections.Generic;
using System.Threading.Tasks;
using Torshify.Origo.Contracts.V1;

namespace Torshify.Radio.TrackProviders
{
    public interface ITrackProvider
    {
        Task<IEnumerable<Track>> GetNextTrackBatch();
    }
}