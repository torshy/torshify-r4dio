using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.FavoriteHandlers
{
    public class TrackListFavoriteHandler : FavoriteHandler<TrackStreamFavorite>
    {
        #region Methods

        public override bool CanHandleFavorite(Favorite favorite)
        {
            TrackStreamFavorite streamFavorite = favorite as TrackStreamFavorite;

            if (streamFavorite != null)
            {
                return streamFavorite.StreamData is TrackListStreamData;
            }

            return false;
        }

        protected override void Play(TrackStreamFavorite favorite)
        {
            var data = favorite.StreamData as TrackListStreamData;

            if (data != null)
            {
                Radio.Play(data.Tracks.ToTrackStream("Favorites"));
            }
            else
            {
                ToastService.Show("Unable to play favorite");
            }
        }

        protected override void Queue(TrackStreamFavorite favorite)
        {
            var data = favorite.StreamData as TrackListStreamData;

            if (data != null)
            {
                Radio.Queue(data.Tracks.ToTrackStream("Favorites"));
            }
            else
            {
                ToastService.Show("Unable to queue favorite");
            }
        }

        #endregion Methods
    }
}