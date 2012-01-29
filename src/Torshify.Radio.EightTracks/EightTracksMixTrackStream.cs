using System;
using System.Collections;
using System.Collections.Generic;

using EightTracks;

using Torshify.Radio.EightTracks.Converters;
using Torshify.Radio.Framework;

using Track = Torshify.Radio.Framework.Track;

namespace Torshify.Radio.EightTracks
{
    public class EightTracksMixTrackStream : ITrackStream
    {
        #region Fields

        private readonly Mix _startMix;

        private Mix _currentMix;
        private PlayResponse _currentPlayResponse;
        private PlayTokenResponse _playToken;

        #endregion Fields

        #region Constructors

        public EightTracksMixTrackStream(Mix startMix)
        {
            _startMix = startMix;
            _currentMix = startMix;
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<Track> Current
        {
            get
            {
                if (_currentPlayResponse == null)
                {
                    return null;
                }

                var imageUrlConverter = new MixToImageConverter();

                return new[]
                {
                    new EightTracksTrack
                    {
                        AlbumArt = (string)imageUrlConverter.Convert(_currentMix, null, null, null),
                        Artist = _currentPlayResponse.Set.Track.Performer,
                        Name = _currentPlayResponse.Set.Track.Name,
                        TrackId = _currentPlayResponse.Set.Track.ID,
                        Uri = new Uri(_currentPlayResponse.Set.Track.Url, UriKind.Absolute),
                        TokenId = _playToken.PlayToken,
                        MixId = _currentMix.ID
                    }
                };
            }
        }

        public bool SupportsTrackSkipping
        {
            get { return false; }
        }

        public bool MoveToNextSimilarMixAtEnd
        {
            get; 
            set;
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            _playToken = null;
            _currentPlayResponse = null;
        }

        public bool MoveNext()
        {
            using (var session = new EightTracksSession(EightTracksModule.ApiKey))
            {
                if (_playToken == null)
                {
                    _playToken = session.Query<Play>().GetPlayToken();
                }

                if (_currentPlayResponse == null)
                {
                    _currentPlayResponse = session.Query<Play>().Execute(_playToken.PlayToken, _currentMix.ID);
                }
                else if (!_currentPlayResponse.Set.AtEnd)
                {
                    _currentPlayResponse = session.Query<Play>().Next(_playToken.PlayToken, _currentMix.ID);
                }

                if (MoveToNextSimilarMixAtEnd)
                {
                    if (_currentPlayResponse.Set == null || _currentPlayResponse.Set.AtEnd)
                    {
                        if (_currentMix != null)
                        {
                            var nextMixResponse = session.Query<Mixes>().GetNextMix(_playToken.PlayToken, _currentMix.ID);
                            _currentMix = nextMixResponse.NextMix;

                            if (_currentMix != null)
                            {
                                _currentPlayResponse = session.Query<Play>().Execute(_playToken.PlayToken,
                                                                                     _currentMix.ID);
                            }
                        }

                        // TODO : Add user-notification and logging if there is any errors
                    }
                }

                if (_currentPlayResponse.Set == null)
                {
                    return false;
                }
            }

            return _currentPlayResponse != null && !_currentPlayResponse.Set.AtEnd;
        }

        public void Reset()
        {
            _currentMix = _startMix;
            _playToken = null;
            _currentPlayResponse = null;
        }

        #endregion Methods
    }
}