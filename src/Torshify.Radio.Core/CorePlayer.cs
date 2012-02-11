using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core
{
    [Export("CorePlayer", typeof(ITrackPlayer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CorePlayer : NotificationObject, ITrackPlayer, IPartImportsSatisfiedNotification
    {
        #region Fields

        private readonly Dictionary<string, double> _volumeMap;

        private bool _isMuted;

        #endregion Fields

        #region Constructors

        public CorePlayer()
        {
            _volumeMap = new Dictionary<string, double>();
        }

        #endregion Constructors

        #region Events

        public event EventHandler<TrackBufferingEventArgs> BufferingProgressChanged;

        public event EventHandler IsPlayingChanged;

        public event EventHandler<TrackEventArgs> TrackComplete;

        public event EventHandler<TrackProgressEventArgs> TrackProgress;

        #endregion Events

        #region Properties

        public double BufferingProgress
        {
            get
            {
                if (CurrentPlayer != null)
                {
                    return CurrentPlayer.Value.BufferingProgress;
                }

                return 1.0;
            }
        }

        public bool CanSeek
        {
            get
            {
                if (CurrentPlayer != null)
                {
                    return CurrentPlayer.Value.CanSeek;
                }

                return false;
            }
        }

        public Lazy<ITrackPlayer, ITrackPlayerMetadata> CurrentPlayer
        {
            get;
            private set;
        }

        public bool IsBuffering
        {
            get
            {
                if (CurrentPlayer != null)
                {
                    return CurrentPlayer.Value.IsBuffering;
                }

                return false;
            }
        }

        public bool IsMuted
        {
            get { return _isMuted; }
            set
            {
                _isMuted = value;

                foreach (var player in TrackPlayers)
                {
                    player.Value.IsMuted = _isMuted;
                }

                RaisePropertyChanged("IsMuted");
            }
        }

        public bool IsPlaying
        {
            get
            {
                return TrackPlayers.Any(p => p.Value.IsPlaying);
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (CurrentPlayer != null)
                {
                    return CurrentPlayer.Value.Position;
                }

                return TimeSpan.Zero;
            }
            set
            {
                if (CurrentPlayer != null && CurrentPlayer.Value.CanSeek)
                {
                    CurrentPlayer.Value.Position = value;
                }

                RaisePropertyChanged("Position");
            }
        }

        [ImportMany]
        public IEnumerable<Lazy<ITrackPlayer, ITrackPlayerMetadata>> TrackPlayers
        {
            get;
            set;
        }

        public double Volume
        {
            get
            {
                if (CurrentPlayer != null)
                {
                    return _volumeMap[CurrentPlayer.Metadata.Name];
                }

                return 0.5;
            }
            set
            {
                if (CurrentPlayer != null)
                {
                    _volumeMap[CurrentPlayer.Metadata.Name] = value;
                    CurrentPlayer.Value.Volume = value;
                }

                RaisePropertyChanged("Volume");
            }
        }

        #endregion Properties

        #region Methods

        public bool CanPlay(Track track)
        {
            return TrackPlayers.Any(p => p.Value.CanPlay(track));
        }

        public void Load(Track track)
        {
            if (CurrentPlayer != null)
            {
                CurrentPlayer.Value.Stop();
            }

            var player = TrackPlayers.FirstOrDefault(p => p.Value.CanPlay(track));

            if (player != null)
            {
                CurrentPlayer = player;
                CurrentPlayer.Value.Load(track);
            }
        }

        public void OnImportsSatisfied()
        {
            foreach (var player in TrackPlayers)
            {
                _volumeMap[player.Metadata.Name] = 0.5;

                player.Value.BufferingProgressChanged += PlayerBufferingProgressChanged;
                player.Value.IsPlayingChanged += PlayerIsPlayingChanged;
                player.Value.TrackComplete += PlayerTrackComplete;
                player.Value.TrackProgress += PlayerTrackProgress;
            }
        }

        public void Pause()
        {
            if (CurrentPlayer != null)
            {
                CurrentPlayer.Value.Pause();
            }
        }

        public void Play()
        {
            if (CurrentPlayer != null)
            {
                CurrentPlayer.Value.Volume = Volume;
                CurrentPlayer.Value.Play();
            }
        }

        public void Stop()
        {
            if (CurrentPlayer != null)
            {
                CurrentPlayer.Value.Stop();
            }
        }

        public void Initialize()
        {
            foreach (var trackPlayer in TrackPlayers)
            {
                trackPlayer.Value.Initialize();
            }
        }

        public void Dispose()
        {
            foreach (var trackPlayer in TrackPlayers)
            {
                trackPlayer.Value.Dispose();
            }
        }

        private void PlayerBufferingProgressChanged(object sender, TrackBufferingEventArgs e)
        {
            var handler = BufferingProgressChanged;

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private void PlayerIsPlayingChanged(object sender, EventArgs e)
        {
            var handler = IsPlayingChanged;

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private void PlayerTrackComplete(object sender, TrackEventArgs e)
        {
            var handler = TrackComplete;

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private void PlayerTrackProgress(object sender, TrackProgressEventArgs e)
        {
            var handler = TrackProgress;

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        #endregion Methods
    }
}