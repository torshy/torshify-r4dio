using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Hot
{
    public class TopHotttFavoriteHandler : FavoriteHandler<TrackStreamFavorite>
    {
        #region Methods

        public override bool CanHandleFavorite(Favorite favorite)
        {
            TrackStreamFavorite streamFavorite = favorite as TrackStreamFavorite;

            if (streamFavorite != null)
            {
                return streamFavorite.StreamData is TopHotttTrackStreamData;
            }

            return false;
        }

        protected override void Play(TrackStreamFavorite favorite)
        {
            var data = favorite.StreamData as TopHotttTrackStreamData;

            if (data != null)
            {
                Radio.Play(new TopHotttTrackStream(Radio));
            }
            else
            {
                ToastService.Show("Unable to play favorite");
            }
        }

        protected override void Queue(TrackStreamFavorite favorite)
        {
            var data = favorite.StreamData as TopHotttTrackStreamData;

            if (data != null)
            {
                Radio.Queue(new TopHotttTrackStream(Radio));
            }
            else
            {
                ToastService.Show("Unable to queue favorite");
            }
        }

        #endregion Methods
    }
}