using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.FavoriteHandlers
{
    public class TrackContainerFavoriteHandler : FavoriteHandler<TrackContainerFavorite>
    {
        public override bool CanHandleFavorite(Favorite favorite)
        {
            return favorite is TrackContainerFavorite;
        }

        protected override void Play(TrackContainerFavorite favorite)
        {
            Radio.Play(favorite.TrackContainer.Tracks.ToTrackStream("Favorites"));
        }

        protected override void Queue(TrackContainerFavorite favorite)
        {
            Radio.Queue(favorite.TrackContainer.Tracks.ToTrackStream("Favorites"));
        }
    }
}