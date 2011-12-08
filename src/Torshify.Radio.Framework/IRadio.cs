using System;
using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface IRadio : IRadioTrackSource, IRadioTrackPlayer
    {
        #region Properties

        IRadioStationContext CurrentContext
        {
            get;
        }

        Lazy<IRadioStation, IRadioStationMetadata> CurrentStation
        {
            get;
        }

        Lazy<IRadioTrackPlayer, IRadioTrackPlayerMetadata> CurrentPlayer
        {
            get;
        }

        IEnumerable<Lazy<IRadioStation, IRadioStationMetadata>> Stations
        {
            get;
        }

        IEnumerable<Lazy<IRadioTrackSource, IRadioTrackSourceMetadata>> TrackSources
        {
            get;
        }

        IEnumerable<Lazy<IRadioTrackPlayer, IRadioTrackPlayerMetadata>> TrackPlayers
        {
            get;
        }

        bool HasCurrentTrack
        {
            get;
        }

        RadioTrack CurrentTrack
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