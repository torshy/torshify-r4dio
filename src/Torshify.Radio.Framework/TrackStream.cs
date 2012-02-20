using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Framework
{
    public class TrackStream : NotificationObject, ITrackStream
    {
        #region Fields

        private readonly IEnumerator _enumerator;
        private readonly List<IEnumerable<Track>> _tracks;
        private IEnumerable<Track> _current;
        private string _description;
        private Func<TrackStreamData> _streamData;

        #endregion Fields

        #region Constructors

        public TrackStream(IEnumerable<Track> tracks, string description, string source)
            : this(tracks, description, () => null)
        {
            _streamData = () =>
            {
                _enumerator.Reset();

                var track = Current.FirstOrDefault(i => !string.IsNullOrEmpty(i.AlbumArt));

                return new TrackListStreamData
                {
                    Name = "Playlist",
                    Image = track != null ? track.AlbumArt : null,
                    Source = source,
                    Description = Description,
                    Tracks = Current.ToArray()
                };
            };
        }

        public TrackStream(IEnumerable<Track> tracks, string description, Func<TrackStreamData> streamData)
        {
            _current = new Track[0];
            _tracks = new List<IEnumerable<Track>>(new[] { tracks });
            _enumerator = _tracks.GetEnumerator();

            _description = description;
            _streamData = streamData;

            SupportsTrackSkipping = true;
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<Track> Current
        {
            get
            {
                return _current;
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
                return _streamData();
            }
        }

        public Func<TrackStreamData> DataGenerator
        {
            get
            {
                return _streamData;
            }
            set
            {
                _streamData = value;
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
            var result = _enumerator.MoveNext();
            if (result)
            {
                _current = (IEnumerable<Track>)_enumerator.Current;
            }

            return result;
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

        public static ITrackStream ToTrackStream(this Track track, string description, string source)
        {
            return new TrackStream(new[] { track }, description, source);
        }

        public static ITrackStream ToTrackStream(this IEnumerable<Track> tracks, string description = null, string source = null)
        {
            return new TrackStream(tracks.ToArray(), description, source);
        }

        #endregion Methods
    }
}