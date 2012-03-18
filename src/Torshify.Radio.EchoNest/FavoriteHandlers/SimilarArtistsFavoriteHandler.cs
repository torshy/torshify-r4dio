using Torshify.Radio.EchoNest.Views.Similar.Tabs;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.FavoriteHandlers
{
    public class SimilarArtistsFavoriteHandler : FavoriteHandler<TrackStreamFavorite>
    {
        #region Methods

        public override bool CanHandleFavorite(Favorite favorite)
        {
            TrackStreamFavorite streamFavorite = favorite as TrackStreamFavorite;

            if (streamFavorite != null)
            {
                return streamFavorite.StreamData is SimilarArtistsTrackStreamData;
            }

            return false;
        }

        protected override void Play(TrackStreamFavorite favorite)
        {
            var data = favorite.StreamData as SimilarArtistsTrackStreamData;

            if (data != null)
            {
                Radio.Play(new SimilarArtistsTrackStream(Radio, data.Artists));
            }
            else
            {
                ToastService.Show("Unable to play favorite");
            }
        }

        protected override void Queue(TrackStreamFavorite favorite)
        {
            var data = favorite.StreamData as SimilarArtistsTrackStreamData;

            if (data != null)
            {
                Radio.Queue(new SimilarArtistsTrackStream(Radio, data.Artists));
            }
            else
            {
                ToastService.Show("Unable to queue favorite");
            }
        }

        #endregion Methods
    }
}