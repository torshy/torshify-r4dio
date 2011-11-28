using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torshify.Radio.Framework
{
    public interface IRadioStationContext
    {
        Task SetTrackProvider(Func<IEnumerable<IRadioTrack>> getNextBatchProvider);
        void SetView(ViewData viewData);
        void GoToTracks();
        void GoToStations();
    }
}