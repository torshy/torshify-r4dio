using System;
using System.ComponentModel.Composition;
using System.Net;

using Grooveshark_Sharp;

using Microsoft.Practices.Prism.Logging;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Grooveshark
{
    [RadioTrackPlayerMetadata(Name = "Grooveshark", Icon = "pack://siteoforigin:,,,/Resources/Icons/Grooveshark_Logo.png")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GroovesharkRadioTrackPlayer : IRadioTrackPlayer
    {
        #region Fields

        private readonly ILoggerFacade _log;

        private bool _isBuffering;
        private bool _isPlaying;
        private GrooveSharkRadioTrackPlayerHandler _trackPlayer;
        private float _volume = 0.5f;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public GroovesharkRadioTrackPlayer(ILoggerFacade log)
        {
            _log = log;
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsBufferingChanged;

        public event EventHandler IsPlayingChanged;

        public event EventHandler<TrackEventArgs> TrackComplete;

        public event EventHandler<TrackProgressEventArgs> TrackProgress;

        #endregion Events

        #region Properties

        public bool IsBuffering
        {
            get { return _isBuffering; }
            private set
            {
                _isBuffering = value;
                OnIsBufferingChanged();
            }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnIsPlayingChanged();
                }
            }
        }

        public float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;

                if (_trackPlayer != null)
                {
                    _trackPlayer.Volume = value;
                }
            }
        }

        #endregion Properties

        #region Methods

        public bool CanPlay(RadioTrack track)
        {
            return track is GroovesharkRadioTrack;
        }

        public void Initialize()
        {
        }

        public void Load(RadioTrack track)
        {
            var currentTrack = track as GroovesharkRadioTrack;

            if (currentTrack != null && GroovesharkRadioTrackSource.Session != null)
            {
                var streaming = new GroovesharkStreaming(GroovesharkRadioTrackSource.Session);
                GroovesharkStreamingResult[] key = null;

                try
                {
                    key = streaming.GetStreamingKey(currentTrack.SongID);
                }
                catch (GroovesharkException e)
                {
                    _log.Log(e.Message + " , code: " + e.Code, Category.Exception, Priority.High);

                    GroovesharkRadioTrackSource.Session = new GroovesharkSession();
                    GroovesharkRadioTrackSource.Session.Connect();

                    streaming = new GroovesharkStreaming(GroovesharkRadioTrackSource.Session);
                    key = streaming.GetStreamingKey(currentTrack.SongID);
                }

                if (key == null || key.Length == 0)
                {
                    throw new Exception("Unable to load track");
                }

                var url = streaming.GetStreamingUrl(key);
                var webRequest = (HttpWebRequest)WebRequest.Create(url);

                try
                {
                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                    Action<RadioTrack> trackComplete = OnTrackComplete;
                    Action<bool> isPlaying = parameter => IsPlaying = parameter;
                    Action<bool> isBuffering = parameter => IsBuffering = parameter;
                    Action<double, double> trackProgress = OnTrackProgress;
                    _trackPlayer = new GrooveSharkRadioTrackPlayerHandler(
                        _log,
                        currentTrack,
                        trackComplete,
                        isPlaying,
                        isBuffering,
                        trackProgress);

                    _trackPlayer.Initialize(response);
                }
                catch (WebException e)
                {
                    _log.Log(url + ": " + e, Category.Exception, Priority.Medium);
                    throw;
                }
            }
        }

        public void Pause()
        {
            if (_trackPlayer != null)
            {
                _trackPlayer.Pause();
            }
        }

        public void Play()
        {
            if (_trackPlayer != null)
            {
                _trackPlayer.Play();
            }
        }

        public void Stop()
        {
            if (_trackPlayer != null)
               {
               _trackPlayer.Stop();
               }
        }

        protected virtual void OnTrackComplete(RadioTrack currentTrack)
        {
            var handler = TrackComplete;

            if (handler != null)
            {
                handler(this, new TrackEventArgs(this, currentTrack));
            }
        }

        protected virtual void OnTrackProgress(double totalMilliseconds, double elapsedMilliseconds)
        {
            var handler = TrackProgress;

            if (handler != null)
            {
                handler(this, new TrackProgressEventArgs(this, totalMilliseconds, elapsedMilliseconds));
            }
        }

        private void OnIsBufferingChanged()
        {
            var handler = IsBufferingChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnIsPlayingChanged()
        {
            var handler = IsPlayingChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}