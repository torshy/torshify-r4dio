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
    [RadioTrackSourceMetadata(Name = "Spotify")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SpotifyRadioTrackSource : IRadioTrackSource, LoginServiceCallback
    {
        #region Fields

        private readonly ILoggerFacade _logger;

        #endregion Fields

        #region Constructors

        static SpotifyRadioTrackSource()
        {
            OrigoConnectionManager.Instance.Initialize();
        }

        [ImportingConstructor]
        public SpotifyRadioTrackSource(ILoggerFacade logger)
        {
            _logger = logger;
        }

        #endregion Constructors

        #region Methods

        public IEnumerable<RadioTrackContainer> GetAlbumsByArtist(string artist)
        {
            List<RadioTrackContainer> albums = new List<RadioTrackContainer>();

            if (!IsLoggedIn())
            {
                return albums;
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
                        RadioTrackContainer container = new RadioTrackContainer();
                        container.ArtistName = artist;
                        container.Tracks = albumGroup.Select(SpotifyRadioTrackPlayer.ConvertTrack).ToArray();

                        var firstOrDefault = container.Tracks.FirstOrDefault();
                        if (firstOrDefault != null)
                        {
                            container.Name = firstOrDefault.Album;
                            container.ContainerArt = container.ContainerArt = firstOrDefault.AlbumArt;
                        }

                        albums.Add(container);
                    }
                }

                query.Close();
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, Category.Exception, Priority.Medium);
                query.Abort();
            }

            return albums;
        }

        public IEnumerable<RadioTrack> GetTracksByArtist(string artist, int offset, int count)
        {
            IEnumerable<RadioTrack> tracks = new RadioTrack[0];

            if (!IsLoggedIn())
            {
                return tracks;
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
                    tracks = browse.Tracks
                        .Where(t => t.IsAvailable)
                        .Skip(offset)
                        .Take(count)
                        .Select(SpotifyRadioTrackPlayer.ConvertTrack).ToArray();
                }

                query.Close();
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, Category.Exception, Priority.Medium);
                query.Abort();
            }

            return tracks;
        }

        public IEnumerable<RadioTrack> GetTracksByName(string name, int offset, int count)
        {
            IEnumerable<RadioTrack> tracks = new RadioTrack[0];

            if (!IsLoggedIn())
            {
                return tracks;
            }

            QueryServiceClient query = new QueryServiceClient();

            try
            {
                QueryResult result = query.Query(name, offset, count, 0, 0, 0, 0);
                tracks = result.Tracks
                    .Where(t => t.IsAvailable)
                    .Select(SpotifyRadioTrackPlayer.ConvertTrack)
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

        public void Initialize()
        {
        }

        void LoginServiceCallback.OnLoggedIn()
        {
        }

        void LoginServiceCallback.OnLoggedOut()
        {
        }

        void LoginServiceCallback.OnLoginError(string message)
        {
        }

        void LoginServiceCallback.OnPing()
        {
        }

        private bool IsLoggedIn()
        {
            LoginServiceClient client = new LoginServiceClient(new InstanceContext(this));

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
    }
}