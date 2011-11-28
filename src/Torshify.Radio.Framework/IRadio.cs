using System;
using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface IRadio : IRadioTrackSource, IRadioTrackPlayer
    {
        #region Properties

        Lazy<IRadioStation, IRadioStationMetadata> CurrentStation
        {
            get;
        }

        IEnumerable<Lazy<IRadioStation, IRadioStationMetadata>> Stations
        {
            get;
        }

        IEnumerable<IRadioTrackSource> TrackSources
        {
            get;
        }

        IEnumerable<IRadioTrackPlayer> TrackPlayers
        {
            get;
        }

        bool HasCurrentTrack
        {
            get;
        }

        IRadioTrack CurrentTrack
        {
            get;
        }

        TimeSpan CurrentTrackElapsed
        {
            get;
        }

        object GetService(Type serviceType);

        void TuneIn(Lazy<IRadioStation, IRadioStationMetadata> radioStation);

        #endregion Properties
    }
}