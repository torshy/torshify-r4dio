using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.LoveHate
{
    public class LoveHateFavoriteHandler : FavoriteHandler<TrackStreamFavorite>
    {
        #region Methods

        public override bool CanHandleFavorite(Favorite favorite)
        {
            TrackStreamFavorite streamFavorite = favorite as TrackStreamFavorite;

            if (streamFavorite != null)
            {
                return streamFavorite.StreamData is LoveHateTrackStreamData;
            }

            return false;
        }

        protected override void Play(TrackStreamFavorite favorite)
        {
            var data = favorite.StreamData as LoveHateTrackStreamData;

            if (data != null)
            {
                Radio.Play(new LoveHateTrackStream(data.InitialArtist, Radio, Logger, ToastService));
            }
            else
            {
                ToastService.Show("Unable to play favorite");
            }
        }

        protected override void Queue(TrackStreamFavorite favorite)
        {
            var data = favorite.StreamData as LoveHateTrackStreamData;

            if (data != null)
            {
                Radio.Queue(new LoveHateTrackStream(data.InitialArtist, Radio, Logger, ToastService));
            }
            else
            {
                ToastService.Show("Unable to queue favorite");
            }
        }

        #endregion Methods
    }
}