using System;
using System.Globalization;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks
{
    public class EightTracksTrack : Track
    {
        #region Properties

        public int MixId
        {
            get; set;
        }

        public int TokenId
        {
            get; set;
        }

        public int TrackId
        {
            get; set;
        }

        public Uri Uri
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override TrackLink ToLink()
        {
            TrackLink link = new TrackLink("8tracks");
            link["MixId"] = MixId.ToString(CultureInfo.InvariantCulture);
            link["TokenId"] = TokenId.ToString(CultureInfo.InvariantCulture);
            link["TrackId"] = TrackId.ToString(CultureInfo.InvariantCulture);
            return link;
        }

        #endregion Methods
    }
}