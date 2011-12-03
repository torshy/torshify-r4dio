using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using EightTracks;
using Torshify.Radio.EightTracks.Converters;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks
{
    public class EightTracksTrackManager
    {
        #region Fields

        private static readonly EightTracksTrackManager _instance = new EightTracksTrackManager();

        private IRadioStationContext _context;
        private Mix _currentMix;
        private PlayResponse _currentPlayResponse;
        private PlayTokenResponse _playToken;

        #endregion Fields

        #region Properties

        public static EightTracksTrackManager Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion Properties

        #region Methods

        public void Deinitialize()
        {
            _context = null;
            _currentPlayResponse = null;
            _currentMix = null;
        }

        public void Initialize(IRadioStationContext context)
        {
            _context = context;
        }

        public void StartMix(Mix mix)
        {
            _currentMix = mix;
            _currentPlayResponse = null;

            using (var session = new EightTracksSession(EightTracksRadioStation.ApiKey))
            {
                if (_playToken == null)
                {
                    _playToken = session.Query<Play>().GetPlayToken();
                }

                _context
                    .SetTrackProvider(GetTrackFactory)
                    .ContinueWith(
                        t => _context.GoToTracks(),
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private IEnumerable<RadioTrack> GetTrackFactory()
        {
            if (_currentMix == null || _playToken == null)
            {
                return new RadioTrack[0];
            }

            using (var session = new EightTracksSession(EightTracksRadioStation.ApiKey))
            {
                if (_currentPlayResponse == null)
                {
                    _currentPlayResponse = session.Query<Play>().Execute(_playToken.PlayToken, _currentMix.ID);
                }
                else if (!_currentPlayResponse.Set.AtEnd)
                {
                    _currentPlayResponse = session.Query<Play>().Next(_playToken.PlayToken, _currentMix.ID);
                }
                
                if (_currentPlayResponse.Set.AtEnd)
                {
                    var nextMixResponse = session.Query<Mixes>().GetNextMix(_playToken.PlayToken, _currentMix.ID);
                    _currentMix = nextMixResponse.NextMix;
                    _currentPlayResponse = session.Query<Play>().Execute(_playToken.PlayToken, _currentMix.ID);
                }
            }

            MixToImageConverter imageUrlConverter = new MixToImageConverter();

            return new RadioTrack[]
                       {
                           new EightTracksRadioTrack
                               {
                                   AlbumArt = (string)imageUrlConverter.Convert(_currentMix, null, null, null),
                                   Artist = _currentPlayResponse.Set.Track.Performer,
                                   Name = _currentPlayResponse.Set.Track.Name,
                                   Track = _currentPlayResponse.Set.Track,
                                   Uri = new Uri(_currentPlayResponse.Set.Track.Url, UriKind.Absolute),
                                   TokenID = _playToken.PlayToken,
                                   MixID = _currentMix.ID
                               }
                       };
        }

        #endregion Methods
    }
}