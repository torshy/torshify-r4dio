using System.Collections.Generic;
using System.ComponentModel.Composition;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Grooveshark
{
    [TrackSourceMetadata(Name = "Grooveshark")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GroovesharkTrackSource : ITrackSource
    {
        public IEnumerable<Track> GetTracksByName(string name)
        {
            return new Track[0];
        }

        public IEnumerable<TrackContainer> GetAlbumsByArtist(string artist)
        {
            return new TrackContainer[0];
        }

        public bool SupportsLink(TrackLink trackLink)
        {
            return trackLink.TrackSource == "grooveshark";
        }

        public Track FromLink(TrackLink trackLink)
        {
            string songIdAsString = trackLink["SongID"];

            int songId;

            if (int.TryParse(songIdAsString, out songId))
            {
                return new GroovesharkTrack(songId);
            }

            return null;
        }
    }
}