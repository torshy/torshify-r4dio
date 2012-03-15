using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EchoNest;
using EchoNest.Playlist;
using EchoNest.Song;

using Torshify.Radio.EchoNest.Views.Similar.Tabs;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Style
{
    public class StyleTrackStream : ITrackStream
    {
        #region Fields

        private readonly StaticArgument _argument;
        private readonly IRadio _radio;
        private readonly IToastService _toastService;

        private Track[] _currentTracks;
        private Queue<SongBucketItem> _songQueue;

        #endregion Fields

        #region Constructors

        public StyleTrackStream(
            StaticArgument argument,
            IRadio radio,
            IToastService toastService)
        {
            _argument = argument;
            _radio = radio;
            _toastService = toastService;
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
                return "Customised playlist";
            }
        }

        public TrackStreamData Data
        {
            get
            {
                return new SimilarArtistsTrackStreamData();
            }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
        }

        public bool MoveNext(CancellationToken token)
        {
            if (_songQueue == null)
            {
                using (var session = new EchoNestSession(EchoNestModule.ApiKey))
                {
                    var response = session.Query<Static>().Execute(_argument);

                    if (response == null)
                    {
                        // Try desperatly once more to see if its really not possible to get a proper response
                        response = session.Query<Static>().Execute(_argument);
                    }

                    if (response == null)
                    {
                        _toastService.Show("Trouble getting customized playlist from EchoNest");
                        return false;
                    }

                    if (response.Status.Code == ResponseCode.Success)
                    {
                        _songQueue = new Queue<SongBucketItem>(response.Songs);
                    }
                    else
                    {
                        _songQueue = new Queue<SongBucketItem>();
                    }
                }
            }

            while (_songQueue.Count > 0)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                var song = _songQueue.Dequeue();

                var queryResult = _radio.GetTracksByName(song.ArtistName + " " + song.Title).ToArray();

                if (!queryResult.Any())
                {
                    queryResult = _radio.GetTracksByName(song.ArtistName).ToArray();
                }

                _currentTracks = queryResult.Take(1).ToArray();

                if (_currentTracks.Any())
                {
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            _currentTracks = null;
            _songQueue = null;
        }

        #endregion Methods
    }
}