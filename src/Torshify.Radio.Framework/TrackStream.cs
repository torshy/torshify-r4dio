using System.Collections;
using System.Collections.Generic;

using Microsoft.Practices.Prism.ViewModel;
using System.Linq;

namespace Torshify.Radio.Framework
{
    public class TrackStream : NotificationObject, ITrackStream
    {
        #region Fields

        private readonly IEnumerator _enumerator;

        private string _description;
        private List<IEnumerable<Track>> _tracks;

        #endregion Fields

        #region Constructors

        public TrackStream(IEnumerable<Track> tracks)
        {
            _tracks = new List<IEnumerable<Track>>(new[] { tracks });
            _enumerator = _tracks.GetEnumerator();



            SupportsTrackSkipping = true;
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<Track> Current
        {
            get
            {
                return _enumerator.Current as IEnumerable<Track>;
            }
        }

        public bool SupportsTrackSkipping
        {
            get;
            set;
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        public TrackStreamData Data
        {
            get
            {
                var track = Current.FirstOrDefault(i => !string.IsNullOrEmpty(i.AlbumArt));

                return new TrackListStreamData()
                {
                    Name = "Playlist",
                    Image = track != null ? track.AlbumArt : null,
                    Description = Description,
                    Tracks = Current.ToArray()
                };
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        #endregion Methods
    }

    public static class TrackStreamExtensions
    {
        #region Methods

        public static ITrackStream ToTrackStream(this IEnumerable<Track> tracks, string description = null)
        {
            return new TrackStream(tracks)
            {
                Description = description
            };
        }

        #endregion Methods
    }
}