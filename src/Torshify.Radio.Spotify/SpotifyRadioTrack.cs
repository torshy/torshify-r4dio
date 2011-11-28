using System;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Spotify
{
    public class SpotifyRadioTrack : NotificationObject, IRadioTrack
    {
        #region Fields

        private string _album;
        private string _albumArt;
        private string _artist;
        private string _name;
        private TimeSpan _totalDuration;
        private string _trackID;

        #endregion Fields

        #region Properties

        public string Album
        {
            get { return _album; }
            set
            {
                _album = value;
                RaisePropertyChanged("Album");
            }
        }

        public string AlbumArt
        {
            get { return _albumArt; }
            set
            {
                _albumArt = value;
                RaisePropertyChanged("AlbumArt");
            }
        }

        public TimeSpan TotalDuration
        {
            get { return _totalDuration; }
            set
            {
                _totalDuration = value;
                RaisePropertyChanged("TotalDuration");
            }
        }

        public string Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                RaisePropertyChanged("Artist");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string TrackID
        {
            get { return _trackID; }
            set
            {
                _trackID = value;
                RaisePropertyChanged("TrackID");
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