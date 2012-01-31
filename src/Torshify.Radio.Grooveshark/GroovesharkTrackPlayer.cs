using System;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Logging;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Grooveshark
{
    [TrackPlayerMetadata(Name = "Grooveshark")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GroovesharkTrackPlayer : ITrackPlayer
    {
        #region Fields

        private readonly ILoggerFacade _log;

        private bool _isBuffering;
        private bool _isPlaying;
        private GrooveSharkTrackPlayerHandler _trackPlayer;
        private double _volume = 0.5f;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public GroovesharkTrackPlayer(ILoggerFacade log)
        {
            _log = log;
        }

        #endregion Constructors

        #region Events

        public event EventHandler<TrackBufferingEventArgs> BufferingProgressChanged;

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

        public double BufferingProgress
        {
            get { return 0.0; }
        }

        public TimeSpan Position
        {
            get;
            set;
        }

        public bool CanSeek
        {
            get { return false; }
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

        public bool IsMuted
        {
            get;
            set;
        }

        public double Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;

                if (_trackPlayer != null)
                {
                    _trackPlayer.Volume = (float)value;
                }
            }
        }

        #endregion Properties

        #region Methods

        public bool CanPlay(Track track)
        {
            return track is GroovesharkTrack;
        }

        public void Initialize()
        {
        }

        public void Load(Track track)
        {
            var currentTrack = track as GroovesharkTrack;

            if (currentTrack != null)
            {
                try
                {
                    if (_trackPlayer != null)
                    {
                        _trackPlayer.Stop();
                    }
                }
                catch (Exception e)
                {
                    _log.Log("While disposing old trackplayer: " + e.Message, Category.Warn, Priority.Medium);
                }

                var client = GroovesharkModule.GetClient();
                var stream = client.GetMusicStream(currentTrack.SongID, currentTrack.ArtistID);

                Action<Track> trackComplete = OnTrackComplete;
                Action<bool> isPlaying = parameter => IsPlaying = parameter;
                Action<bool> isBuffering = parameter => IsBuffering = parameter;
                Action<double, double> trackProgress = OnTrackProgress;
                _trackPlayer = new GrooveSharkTrackPlayerHandler(
                    _log,
                    currentTrack,
                    trackComplete,
                    isPlaying,
                    isBuffering,
                    trackProgress);

                _trackPlayer.Initialize(stream.Stream);
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

        public void Dispose()
        {
            if (_trackPlayer != null)
            {
                _trackPlayer.Stop();
            }
        }

        protected virtual void OnTrackComplete(Track currentTrack)
        {
            var handler = TrackComplete;

            if (handler != null)
            {
                handler(this, new TrackEventArgs(currentTrack));
            }
        }

        protected virtual void OnTrackProgress(double totalMilliseconds, double elapsedMilliseconds)
        {
            var handler = TrackProgress;

            if (handler != null)
            {
                handler(this, new TrackProgressEventArgs(totalMilliseconds, elapsedMilliseconds));
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