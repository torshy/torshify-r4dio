using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;

using Microsoft.Practices.Prism.Logging;

using Torshify.Origo.Contracts.V1.Query;
using Torshify.Radio.Framework;
using Torshify.Radio.Spotify.LoginService;
using Torshify.Radio.Spotify.QueryService;

namespace Torshify.Radio.Spotify
{
    [TrackSourceMetadata(Name = "Spotify", IconUri = "pack://application:,,,/Torstify.Radio.Spotify;component/Resources/Spotify_Logo.png")]
    public class SpotifyTrackSource : ITrackSource
    {
        #region Fields

        private readonly ILoggerFacade _logger;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public SpotifyTrackSource(ILoggerFacade logger)
        {
            _logger = logger;
        }

        #endregion Constructors

        #region Methods

        public static SpotifyTrack ConvertTrack(Origo.Contracts.V1.Track track)
        {
            string albumArt = null;

            if (!string.IsNullOrEmpty(track.Album.CoverID))
            {
                // TODO : Get location of torshify from config, instead of using localhost :o
                albumArt = "http://localhost:1338/torshify/v1/image/id/" + track.Album.CoverID;
            }

            return new SpotifyTrack
            {
                Index = track.Index,
                TrackId = track.ID,
                Name = track.Name,
                Artist = track.Album.Artist.Name,
                Album = track.Album.Name,
                AlbumArt = albumArt,
                TotalDuration = TimeSpan.FromMilliseconds(track.Duration)
            };
        }

        public IEnumerable<Track> GetTracksByName(string name)
        {
            Track[] tracks = new Track[0];

            if (!IsLoggedIn())
            {
                return tracks;
            }

            QueryServiceClient query = new QueryServiceClient();

            try
            {
                QueryResult result = query.Query(name, 0, 150, 0, 0, 0, 0);

                tracks = result.Tracks
                    .Where(t => t.IsAvailable)
                    .Select(ConvertTrack)
                    .ToArray();

                query.Close();
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, Category.Exception, Priority.Medium);
                query.Abort();
            }

            return tracks;
        }

        public IEnumerable<TrackContainer> GetAlbumsByArtist(string artist)
        {
            List<TrackContainer> containers = new List<TrackContainer>();

            if (!IsLoggedIn())
            {
                return containers;
            }

            QueryServiceClient query = new QueryServiceClient();

            try
            {
                var queryResult = query.Query(artist, 0, 0, 0, 0, 0, 10);
                var result =
                    queryResult.Artists.FirstOrDefault(
                        a => a.Name.Equals(artist, StringComparison.InvariantCultureIgnoreCase));

                if (result == null && queryResult.Artists.Any())
                {
                    result = queryResult.Artists.FirstOrDefault();
                }

                if (result != null)
                {
                    var browse = query.ArtistBrowse(result.ID, ArtistBrowsingType.Full);
                    var albumGroups = browse.Tracks.Where(t => t.IsAvailable).GroupBy(t => t.Album.ID);

                    foreach (var albumGroup in albumGroups)
                    {
                        TrackContainer container = new TrackContainer();
                        container.Owner = new TrackContainerOwner(artist);
                        container.Tracks = albumGroup.Select(SpotifyRadioTrackPlayer.ConvertTrack).ToArray();

                        var firstOrDefault = container.Tracks.FirstOrDefault();
                        if (firstOrDefault != null)
                        {
                            container.Name = firstOrDefault.Album;
                            container.Image = container.Image = firstOrDefault.AlbumArt;
                        }

                        containers.Add(container);
                    }
                }

                query.Close();
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, Category.Exception, Priority.Medium);
                query.Abort();
            }

            return containers;
        }

        public bool SupportsLink(TrackLink trackLink)
        {
            return trackLink.TrackSource == "spotify";
        }

        public Track FromLink(TrackLink trackLink)
        {
            string trackId = trackLink["TrackId"];

            // TODO : Implement properly

            return new SpotifyTrack {TrackId = trackId};
        }

        private bool IsLoggedIn()
        {
            LoginServiceClient client = new LoginServiceClient(new InstanceContext(new NoOpCallbacks()));

            try
            {
                bool result = client.IsLoggedIn();
                client.Close();
                return result;
            }
            catch
            {
                client.Abort();
                return false;
            }
        }

        #endregion Methods

        #region Nested Types

        private class NoOpCallbacks : LoginServiceCallback
        {
            #region Methods

            public void OnLoggedIn()
            {
            }

            public void OnLoginError(string message)
            {
            }

            public void OnLoggedOut()
            {
            }

            public void OnPing()
            {
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}