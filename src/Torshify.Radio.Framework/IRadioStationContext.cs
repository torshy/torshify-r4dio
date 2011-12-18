using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torshify.Radio.Framework
{
    public interface IRadioStationContext
    {
        IRadio Radio { get; }

        Task SetTrackProvider(Func<IEnumerable<RadioTrack>> getNextBatchProvider);
        
        Task SetTrackProvider(TrackProvider trackProvider);
        
        void SetView(ViewData viewData);
        
        void GoToTracks();
        
        void GoToStations();
    }
}