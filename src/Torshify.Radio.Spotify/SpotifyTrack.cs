using Torshify.Radio.Framework;

namespace Torshify.Radio.Spotify
{
    public class SpotifyTrack : Track
    {
        #region Properties

        public string TrackId
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public override TrackLink ToLink()
        {
            TrackLink link = new TrackLink("spotify");
            link["TrackId"] = TrackId;
            return link;
        }

        #endregion Methods
    }
}