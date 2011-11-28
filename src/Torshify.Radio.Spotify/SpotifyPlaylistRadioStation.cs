using System;
using System.Collections.Generic;
using System.Linq;
using Torshify.Origo.Contracts.V1;
using Torshify.Radio.Framework;
using Torshify.Radio.Spotify.QueryService;

namespace Torshify.Radio.Spotify
{
    [RadioStationMetadata(Name = "Spotify playlist", Icon = "MB_0010_tasks.png")]
    public class SpotifyPlaylistRadioStation : IRadioStation
    {
        #region Fields

        private IRadio _radio;

        #endregion Fields

        #region Methods

        public void Initialize(IRadio radio)
        {
            _radio = radio;
        }

        public void OnTunedAway()
        {
        }

        public void OnTunedIn(IRadioStationContext context)
        {
            context.SetTrackProvider(GetPlaylistTracks);
            context.GoToTracks();
        }

        private IEnumerable<IRadioTrack> GetPlaylistTracks()
        {
            var tracks = new List<Track>();
            var query = new QueryServiceClient();

            try
            {
                Playlist result = query.GetPlaylist("spotify:user:spotify:playlist:3Yrvm5lBgnhzTYTXx2l55x");
                tracks.AddRange(result.Tracks);
                query.Close();
            }
            catch (Exception)
            {
                query.Abort();
            }

            return tracks.Select(SpotifyRadioTrackPlayer.ConvertTrack);
        }

        #endregion Methods
    }
}