using System;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Spotify
{
    public class SpotifyRadioTrack : RadioTrack 
    {
        #region Fields

        private string _trackID;

        #endregion Fields

        #region Properties

        public string TrackID
        {
            get { return _trackID; }
            set
            {
                _trackID = value;
                OnPropertyChanged("TrackID");
            }
        }

        #endregion Properties

        #region Methods

        public override bool Equals(object obj)
        {
            SpotifyRadioTrack other = obj as SpotifyRadioTrack;

            if (other != null)
            {
                return other.TrackID.Equals(TrackID);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return TrackID.GetHashCode();
        }

        #endregion Methods
    }
}