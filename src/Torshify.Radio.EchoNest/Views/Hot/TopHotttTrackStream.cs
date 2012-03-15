using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EchoNest;
using EchoNest.Artist;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Hot
{
    public class TopHotttTrackStream : ITrackStream
    {
        #region Fields

        private readonly IRadio _radio;

        private IEnumerable<Track> _currentTracks;
        private int _start;

        #endregion Fields

        #region Constructors

        public TopHotttTrackStream(IRadio radio)
        {
            _start = 0;
            _radio = radio;
            _currentTracks = new Track[0];
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<Track> Current
        {
            get
            {
                return _currentTracks;
            }
        }

        public bool SupportsTrackSkipping
        {
            get
            {
                return true;
            }
        }

        public string Description
        {
            get
            {
                return "Top hot artists";
            }
        }

        public TrackStreamData Data
        {
            get
            {
                return new TopHotttTrackStreamData();
            }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
        }

        public bool MoveNext(CancellationToken token)
        {
            using (var session = new EchoNestSession(EchoNestModule.ApiKey))
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                var response = session.Query<TopHottt>().Execute(2, _start);

                if (response != null && response.Status.Code == ResponseCode.Success)
                {
                    _start += response.Artists.Count;

                    List<Track> tracks = new List<Track>();

                    foreach (var artist in response.Artists.OrderBy(k => Guid.NewGuid()))
                    {
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }

                        var result = _radio
                            .GetTracksByName(artist.Name)
                            .Where(a => a.Artist.Equals(artist.Name, StringComparison.InvariantCultureIgnoreCase))
                            .Take(2);

                        tracks.AddRange(result);
                    }

                    _currentTracks = tracks;

                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            _start = 0;
            _currentTracks = new Track[0];
        }

        #endregion Methods
    }
}