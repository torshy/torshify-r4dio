using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.FavoriteHandlers
{
    public class TrackFavoriteHandler : FavoriteHandler<TrackFavorite>
    {
           public override bool CanHandleFavorite(Favorite favorite)
        {
            return favorite is TrackFavorite;
        }

        protected override void Play(TrackFavorite favorite)
        {
            Radio.Play(new TrackStream(new[] { favorite.Track }, "Favorites", favorite.Track.Artist));
        }

        protected override void Queue(TrackFavorite favorite)
        {
            Radio.Queue(new TrackStream(new[] { favorite.Track }, "Favorites", favorite.Track.Artist));
        }
    }
}