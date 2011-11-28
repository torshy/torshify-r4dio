using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Torshify.Origo.Contracts.V1;
using Torshify.Radio.QueryService;

namespace Torshify.Radio.TrackProviders
{
    public class PlaylistTrackProvider : ITrackProvider
    {
        #region Constructors

        public PlaylistTrackProvider(string playlistLink)
        {
            PlaylistLink = playlistLink;
        }

        #endregion Constructors

        #region Properties

        public string PlaylistLink
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public Task<IEnumerable<Track>> GetNextTrackBatch()
        {
            return Task<IEnumerable<Track>>.Factory.StartNew(GetPlaylistTracks);
        }

        private IEnumerable<Track> GetPlaylistTracks()
        {
            var tracks = new List<Track>();
            var query = new QueryServiceClient();

            try
            {
                Playlist result = query.GetPlaylist(PlaylistLink);
                tracks.AddRange(result.Tracks);
                query.Close();
            }
            catch (Exception)
            {
                query.Abort();
            }

            return tracks;
        }

        #endregion Methods
    }
}