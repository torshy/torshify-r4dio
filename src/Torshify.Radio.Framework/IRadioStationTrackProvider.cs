using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torshify.Radio.Framework
{
    public interface IRadioStationTrackProvider
    {
        #region Methods

        Task<IEnumerable<RadioTrack>> GetNextTracks();

        #endregion Methods
    }
}