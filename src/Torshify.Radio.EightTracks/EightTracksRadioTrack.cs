using EightTracks;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks
{
    public class EightTracksRadioTrack : MediaPlayerRadioTrack
    {
        #region Properties

        public int MixID
        {
            get; set;
        }

        public int TokenID
        {
            get; set;
        }

        public Track Track
        {
            get;  set;
        }

        #endregion Properties

        #region Methods

        public override bool Equals(object obj)
        {
            EightTracksRadioTrack other = obj as EightTracksRadioTrack;

            if (other != null)
            {
                return other.Track.ID.Equals(Track.ID);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Track.ID.GetHashCode();
        }

        #endregion Methods
    }
}