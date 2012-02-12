using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using EightTracks;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.EightTracks.Converters;
using Torshify.Radio.Framework;

using Track = Torshify.Radio.Framework.Track;
using System.Linq;

namespace Torshify.Radio.EightTracks
{
    public class EightTracksMixTrackStream : NotificationObject, ITrackStream
    {
        #region Fields

        private readonly Mix _startMix;
        private readonly IToastService _toastService;

        private Mix _currentMix;
        private PlayResponse _currentPlayResponse;
        private string _description;
        private PlayTokenResponse _playToken;

        #endregion Fields

        #region Constructors

        public EightTracksMixTrackStream(Mix startMix, IToastService toastService)
        {
            _startMix = startMix;
            _toastService = toastService;
            _currentMix = startMix;
            Description = _currentMix.Name;
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
                        Album = _currentMix.Name,
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

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
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

                if (_currentPlayResponse.Errors != null)
                {
                    var errorNodes = _currentPlayResponse.Errors as XmlNode[];

                    if (errorNodes != null && errorNodes.Any())
                    {
                        var errorNode = errorNodes.FirstOrDefault();
                        if (errorNode != null && (errorNode.Name != "nil" && errorNode.Value != "true"))
                        {
                            var errorText = errorNode.InnerText;
                            _toastService.Show(errorText);
                        }
                    }
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
                                Description = _currentMix.Name;

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