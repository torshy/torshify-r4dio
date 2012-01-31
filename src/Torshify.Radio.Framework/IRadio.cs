using System;
using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface IRadio : ITrackSource
    {
        event EventHandler CurrentTrackStreamChanged;

        event EventHandler CurrentTrackChanged;

        event EventHandler UpcomingTrackChanged;

        IEnumerable<Lazy<ITrackSource, ITrackSourceMetadata>> TrackSources
        {
            get;
        }

        IEnumerable<Lazy<ITrackPlayer, ITrackPlayerMetadata>> TrackPlayers
        {
            get;
        }

        ITrackStream CurrentTrackStream
        {
            get;
        }

        Track CurrentTrack
        {
            get;
        }

        Track UpcomingTrack
        {
            get;
        }

        IEnumerable<ITrackStream> TrackStreams
        {
            get;
        }

        bool CanGoToNextTrack
        {
            get;
        }

        void Play(ITrackStream trackStream);

        void Queue(ITrackStream trackStream);

        void NextTrack();
    }
}