using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace Torshify.Radio.Framework
{
    public class MediaPlayerRadioTrackPlayer : IRadioTrackPlayer
    {
        #region Fields

        private bool _isPlaying;
        private Timer _mediaElementProgressTimer;

        #endregion Fields

        #region Constructors

        public MediaPlayerRadioTrackPlayer()
        {
            Player = new MediaPlayer();
            Player.BufferingStarted += OnBufferingStarted;
            Player.BufferingEnded += OnBufferingEnded;
            _mediaElementProgressTimer = new Timer(100);
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
            get
            {
                return Player.IsBuffering;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                _isPlaying = value;
                OnIsPlayingChanged();
            }
        }

        public float Volume
        {
            get { return (float)Application.Current.Dispatcher.Invoke(new Func<float>(() => (float)Player.Volume)); }
            set { Application.Current.Dispatcher.BeginInvoke(new Action<double>(v => Player.Volume = v), value); }
        }

        protected MediaPlayerRadioTrack CurrentTrack
        {
            get;
            set;
        }

        protected TimeSpan CurrentTrackElapsed
        {
            get;
            set;
        }

        protected MediaPlayer Player
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public virtual bool CanPlay(RadioTrack radioTrack)
        {
            return radioTrack is MediaPlayerRadioTrack;
        }

        public virtual void Initialize()
        {
            Player.MediaEnded += OnMediaEnded;
            Player.MediaOpened += OnMediaOpened;

            _mediaElementProgressTimer.Elapsed += OnProgressTimerElapsed;
        }

        public virtual void Load(RadioTrack track)
        {
            var mediaPlayerTrack = track as MediaPlayerRadioTrack;

            if (mediaPlayerTrack != null)
            {
                CurrentTrack = mediaPlayerTrack;
                CurrentTrackElapsed = TimeSpan.Zero;

                Player.Open(mediaPlayerTrack.Uri);
            }
        }

        public void Pause()
        {
            if (Player.CanPause)
            {
                IsPlaying = false;
                Player.Pause();
                _mediaElementProgressTimer.Stop();
            }
        }

        public virtual void Play()
        {
            Player.Play();
            IsPlaying = true;
        }

        public virtual void Stop()
        {
            Player.Stop();
            CurrentTrack = null;
            CurrentTrackElapsed = TimeSpan.Zero;

            _mediaElementProgressTimer.Stop();
        }

        protected virtual void OnIsPlayingChanged()
        {
            var handler = IsPlayingChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnMediaEnded(object sender, EventArgs e)
        {
            _mediaElementProgressTimer.Stop();

            IsPlaying = false;
            OnTrackComplete(CurrentTrack);
            CurrentTrack = null;
        }

        protected virtual void OnMediaOpened(object sender, EventArgs e)
        {
            _mediaElementProgressTimer.Start();
            IsPlaying = true;

            if (Player.NaturalDuration.HasTimeSpan && CurrentTrack.TotalDuration == TimeSpan.Zero)
            {
                CurrentTrack.TotalDuration = Player.NaturalDuration.TimeSpan;
            }
        }

        protected virtual void OnProgressTimerElapsed(object sender, ElapsedEventArgs e)
        {
            CurrentTrackElapsed = CurrentTrackElapsed.Add(TimeSpan.FromMilliseconds(_mediaElementProgressTimer.Interval));
            OnTrackProgress(CurrentTrack.TotalDuration.TotalMilliseconds, CurrentTrackElapsed.TotalMilliseconds);
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

        private void OnBufferingEnded(object sender, EventArgs e)
        {
            OnIsBufferingChanged();
        }

        private void OnBufferingStarted(object sender, EventArgs e)
        {
            OnIsBufferingChanged();
        }

        private void OnIsBufferingChanged()
        {
            var handler = IsBufferingChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}