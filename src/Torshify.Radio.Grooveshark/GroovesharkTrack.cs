using System.Globalization;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Grooveshark
{
    public class GroovesharkTrack : Track
    {
        #region Constructors

        public GroovesharkTrack(int songID, int artistID)
        {
            SongID = songID;
            ArtistID = artistID;
        }

        #endregion Constructors

        #region Properties

        public int SongID
        {
            get;
            private set;
        }

        public int ArtistID
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public override TrackLink ToLink()
        {
            TrackLink link = new TrackLink("gs");
            link["S"] = SongID.ToString(CultureInfo.InvariantCulture);
            link["A"] = ArtistID.ToString(CultureInfo.InvariantCulture);
            link["N"] = Name;
            link["AR"] = Artist;
            return link;
        }

        #endregion Methods
    }
}