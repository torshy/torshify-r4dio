using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Core
{
    [Export(typeof(IRadio))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CoreRadio : IRadio
    {
        #region Properties

        [ImportMany]
        public IEnumerable<Lazy<ITrackPlayer, ITrackPlayerMetadata>> TrackPlayers
        {
            get; set;
        }

        [ImportMany]
        public IEnumerable<Lazy<ITrackSource, ITrackSourceMetadata>> TrackSources
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public Track FromLink(TrackLink trackLink)
        {
            var source = TrackSources.FirstOrDefault(s => s.Value.SupportsLink(trackLink));

            if (source != null)
            {
                return source.Value.FromLink(trackLink);
            }

            return null;
        }

        public IEnumerable<TrackContainer> GetAlbumsByArtist(string artist)
        {
            List<TrackContainer> albums = new List<TrackContainer>();

            foreach (var source in TrackSources)
            {
                albums.AddRange(source.Value.GetAlbumsByArtist(artist));
            }

            return albums;
        }

        public IEnumerable<Track> GetTracksByName(string name)
        {
            List<Track> tracks = new List<Track>();

            foreach (var source in TrackSources)
            {
                tracks.AddRange(source.Value.GetTracksByName(name));
            }

            return tracks;
        }

        public void PlayTrackStream(ITrackStream trackStream)
        {
        }

        public void QueueTrackStream(ITrackStream trackStream)
        {
        }

        public bool SupportsLink(TrackLink trackLink)
        {
            return TrackSources.Any(s => s.Value.SupportsLink(trackLink));
        }

        #endregion Methods
    }
}