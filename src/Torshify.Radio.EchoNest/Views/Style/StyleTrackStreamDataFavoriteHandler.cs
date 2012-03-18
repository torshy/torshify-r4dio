using EchoNest;
using EchoNest.Playlist;
using Torshify.Radio.Framework;
using Term = EchoNest.Term;

namespace Torshify.Radio.EchoNest.Views.Style
{
    public class StyleTrackStreamDataFavoriteHandler : FavoriteHandler<TrackStreamFavorite>
    {
        #region Methods

        public override bool CanHandleFavorite(Favorite favorite)
        {
            TrackStreamFavorite streamFavorite = favorite as TrackStreamFavorite;

            if (streamFavorite != null)
            {
                return streamFavorite.StreamData is StyleTrackStreamData;
            }

            return false;
        }

        protected override void Play(TrackStreamFavorite favorite)
        {
            var data = favorite.StreamData as StyleTrackStreamData;

            if (data != null)
            {
                Radio.Play(new StyleTrackStream(GetArgument(data), Radio, ToastService));
            }
            else
            {
                ToastService.Show("Unable to play favorite");
            }
        }

        protected override void Queue(TrackStreamFavorite favorite)
        {
            var data = favorite.StreamData as StyleTrackStreamData;

            if (data != null)
            {
                Radio.Queue(new StyleTrackStream(GetArgument(data), Radio, ToastService));
            }
            else
            {
                ToastService.Show("Unable to play favorite");
            }
        }

        private StaticArgument GetArgument(StyleTrackStreamData data)
        {
            StaticArgument argument = new StaticArgument();
            argument.MinDanceability = data.MinDanceability;
            argument.MinEnergy = data.MinEnergy;
            argument.MinLoudness = data.MinLoudness;
            argument.MinTempo = data.MinTempo;
            argument.ArtistMinFamiliarity = data.ArtistMinFamiliarity;
            argument.ArtistMinHotttnesss = data.ArtistMinHotttnesss;
            argument.SongMinHotttnesss = data.SongMinHotttnesss;
            argument.Type = data.Type;
            FillTermList(data.Artist, argument.Artist);
            FillTermList(data.Styles, argument.Styles);
            FillTermList(data.Moods, argument.Moods);
            return argument;
        }

        private void FillTermList(Term[] terms, TermList target)
        {
            foreach (var term in terms)
            {
                target.Add(term);
            }
        }

        #endregion Methods
    }
}