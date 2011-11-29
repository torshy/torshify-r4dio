using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Torshify.Origo.Contracts.V1;
using Torshify.Origo.Contracts.V1.Query;
using Torshify.Radio.Framework;
using Torshify.Radio.Spotify.QueryService;

namespace Torshify.Radio.Spotify
{
    [RadioTrackSourceMetadata(Name = "Spotify")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SpotifyRadioTrackSource : IRadioTrackSource
    {
        static SpotifyRadioTrackSource()
        {
            OrigoConnectionManager.Instance.ToString();
        }

        #region Methods

        public IEnumerable<IRadioTrack> GetTracksByAlbum(string artist, string album)
        {
            IEnumerable<IRadioTrack> tracks = new IRadioTrack[0];

            QueryServiceClient query = new QueryServiceClient();

            try
            {
                QueryResult result = query.Query(artist + " " + album, 0, 50, 0, 50, 0, 0);
                Album albumResult = result.Albums.FirstOrDefault(a => a.Artist.Name.Equals(artist, StringComparison.InvariantCultureIgnoreCase) &&
                                                                      a.Name.Equals(album, StringComparison.InvariantCultureIgnoreCase));

                if (albumResult != null)
                {
                    AlbumBrowseResult albumBrowse = query.AlbumBrowse(albumResult.ID);
                    tracks = albumBrowse.Tracks.Select(SpotifyRadioTrackPlayer.ConvertTrack).ToArray();
                }
                else
                {
                    tracks = result.Tracks.Select(SpotifyRadioTrackPlayer.ConvertTrack).ToArray();
                }

                query.Close();
            }
            catch (Exception e)
            {
                query.Abort();
            }

            return tracks;
        }

        public IEnumerable<IRadioTrack> GetTracksByArtist(string artist, int offset, int count)
        {
            IEnumerable<IRadioTrack> tracks = new IRadioTrack[0];

            QueryServiceClient query = new QueryServiceClient();

            try
            {
                QueryResult result = query.Query(artist, offset, count, 0, 0, 0, 0);
                tracks = result.Tracks.Select(SpotifyRadioTrackPlayer.ConvertTrack).ToArray();
                query.Close();
            }
            catch (Exception e)
            {
                query.Abort();
            }

            return tracks;
        }

        public IEnumerable<IRadioTrack> GetTracksByName(string name, int offset, int count)
        {
            IEnumerable<IRadioTrack> tracks = new IRadioTrack[0];

            QueryServiceClient query = new QueryServiceClient();

            try
            {
                QueryResult result = query.Query(name, offset, count, 0, 0, 0, 0);
                tracks = result.Tracks.Select(SpotifyRadioTrackPlayer.ConvertTrack).ToArray();
                query.Close();
            }
            catch (Exception e)
            {
                query.Abort();
            }

            return tracks;
        }

        public void Initialize()
        {
            
        }

        #endregion Methods
    }
}