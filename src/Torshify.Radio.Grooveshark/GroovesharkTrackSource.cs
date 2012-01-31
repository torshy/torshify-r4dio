using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.Practices.Prism.Logging;

using SciLorsGroovesharkAPI.Groove;
using SciLorsGroovesharkAPI.Groove.Functions;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Grooveshark
{
    [TrackSourceMetadata(Name = "Grooveshark")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GroovesharkTrackSource : ITrackSource
    {
        #region Fields

        private readonly ILoggerFacade _logger;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public GroovesharkTrackSource(ILoggerFacade logger)
        {
            _logger = logger;
        }

        #endregion Constructors

        #region Methods

        public IEnumerable<Track> GetTracksByName(string name)
        {
            List<Track> tracks = new List<Track>();

            try
            {
                var response = GroovesharkModule.GetClient().SearchArtist(name);
                var results = response.result.result as List<SearchArtist.SearchArtistResult>;
                tracks.AddRange(Convert(results));
            }
            catch (Exception e)
            {
                _logger.Log("Grooveshark: " + e, Category.Info, Priority.Medium);
            }

            return tracks;
        }

        public IEnumerable<TrackContainer> GetAlbumsByArtist(string artist)
        {
            List<TrackContainer> containers = new List<TrackContainer>();

            try
            {
                var response = GroovesharkModule.GetClient().SearchArtist(artist);
                var results = response.result.result as List<SearchArtist.SearchArtistResult>;
                var byAlbum = results
                    .Where(s => artist.Equals(s.ArtistName, StringComparison.InvariantCultureIgnoreCase))
                    .GroupBy(s => new { Album = s.AlbumName, Year = s.Year });

                foreach (var albumGroup in byAlbum)
                {
                    int year;
                    if (!int.TryParse(albumGroup.Key.Year, out year))
                    {
                        year = DateTime.Now.Year;
                    }

                    TrackContainer container = new TrackContainer
                                               {
                                                   Name = albumGroup.Key.Album,
                                                   Year = year,
                                                   Owner = new TrackContainerOwner(artist),
                                                   Tracks = Convert(albumGroup).ToArray(),
                                               };

                    var firstTrack = container.Tracks.FirstOrDefault();

                    if (firstTrack != null)
                    {
                        container.Image = firstTrack.AlbumArt;
                    }

                    containers.Add(container);
                }
            }
            catch (Exception e)
            {
                _logger.Log("Grooveshark: " + e, Category.Exception, Priority.Medium);
            }

            return containers;
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
                string artistIdAsString = trackLink["ArtistID"];

                int artistId;

                if (int.TryParse(artistIdAsString, out artistId))
                {
                    return new GroovesharkTrack(songId, artistId);
                }
            }

            return null;
        }

        private IEnumerable<Track> Convert(IEnumerable<SearchArtist.SearchArtistResult> songs)
        {
            return songs.Select(result => new GroovesharkTrack(result.SongID, result.AlbumID)
            {
                Name = result.SongName,
                Album = result.AlbumName,
                Artist = result.ArtistName,
                AlbumArt = "http://images.grooveshark.com/static/albums/90_" + result.AlbumID + ".jpg"
            });
        }

        #endregion Methods
    }
}