using System;
using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface IRadio : ITrackSource
    {
        IEnumerable<Lazy<ITrackSource, ITrackSourceMetadata>> TrackSources
        {
            get;
        }

        IEnumerable<Lazy<ITrackPlayer, ITrackPlayerMetadata>> TrackPlayers
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

        void PlayTrackStream(ITrackStream trackStream);

        void QueueTrackStream(ITrackStream trackStream);
    }
}