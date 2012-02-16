using System.Linq;
using System.Threading.Tasks;
using System.Xml;

using EightTracks;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.FavoriteHandlers
{
    public class EightTracksFavoriteHandler : FavoriteHandler<TrackStreamFavorite>
    {
        #region Methods

        public override bool CanHandleFavorite(Favorite favorite)
        {
            TrackStreamFavorite streamFavorite = favorite as TrackStreamFavorite;

            if (streamFavorite != null)
            {
                return streamFavorite.StreamData is EightTracksMixTrackStreamData;
            }

            return false;
        }

        protected override void Play(TrackStreamFavorite favorite)
        {
            var streamData = favorite.StreamData as EightTracksMixTrackStreamData;

            if (streamData != null)
            {
                GetFavoriteMix(streamData)
                    .ContinueWith(task =>
                    {
                        if (task.Result != null)
                        {
                            Radio.Play(new EightTracksMixTrackStream(task.Result, ToastService));
                        }
                    });
            }
            else
            {
                ToastService.Show("Unable to play favorite");
            }
        }

        protected override void Queue(TrackStreamFavorite favorite)
        {
            var streamData = favorite.StreamData as EightTracksMixTrackStreamData;

            if (streamData != null)
            {
                GetFavoriteMix(streamData)
                    .ContinueWith(task =>
                    {
                        if (task.Result != null)
                        {
                            Radio.Queue(new EightTracksMixTrackStream(task.Result, ToastService));
                        }
                    });
            }
            else
            {
                ToastService.Show("Unable to queue favorite");
            }
        }

        private Task<Mix> GetFavoriteMix(EightTracksMixTrackStreamData streamData)
        {
            return Task.Factory
                .StartNew(state =>
                {
                    var d = (EightTracksMixTrackStreamData)state;
                    using (var session = new EightTracksSession(EightTracksModule.ApiKey))
                    {
                        return session.Query<Mixes>().GetMix(d.MixId);
                    }
                }, streamData)
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        ToastService.Show("Error while getting 8track mix");
                    }
                    else
                    {
                        var errorNodes = task.Result.Errors as XmlNode[];

                        if (errorNodes != null && errorNodes.Any())
                        {
                            var errorNode = errorNodes.FirstOrDefault();
                            if (errorNode != null && (errorNode.Name != "nil" && errorNode.Value != "true"))
                            {
                                var errorText = errorNode.InnerText;
                                ToastService.Show(errorText);
                            }
                            else
                            {
                                return task.Result.Mix;
                            }
                        }
                        else if (task.Result.Mix != null)
                        {
                            return task.Result.Mix;
                        }
                        else
                        {
                            ToastService.Show("Unable to find 8tracks mix");
                        }
                    }

                    return null;
                });
        }

        #endregion Methods
    }
}