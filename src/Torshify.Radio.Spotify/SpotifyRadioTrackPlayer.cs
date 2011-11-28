using System;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;

using Torshify.Origo.Contracts.V1;
using Torshify.Radio.Framework;
using Torshify.Radio.Spotify.PlayerControlService;
using Torshify.Radio.Spotify.TrackPlayerService;

namespace Torshify.Radio.Spotify
{
    [Export(typeof(IRadioTrackPlayer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SpotifyRadioTrackPlayer : IRadioTrackPlayer, PlayerControlServiceCallback
    {
        #region Fields

        private PlayerControlServiceClient _controlService;
        private bool _isPlaying;
        private SpotifyRadioTrack _currentTrack;
        private bool _isPaused;

        #endregion Fields

        static SpotifyRadioTrackPlayer()
        {
            OrigoConnectionManager.Instance.ToString();
        }

        #region Events

        public event EventHandler IsPlayingChanged;

        public event EventHandler<TrackEventArgs> TrackComplete;

        public event EventHandler<TrackProgressEventArgs> TrackProgress;

        #endregion Events

        #region Properties

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
            }
        }

        public float Volume
        {
            get
            {
                float volume = 0.5f;

                EnsureControlServiceIsAlive();

                PlayerControlServiceClient controlService = new PlayerControlServiceClient(new InstanceContext(this));
                try
                {
                    volume = controlService.GetVolume();
                    controlService.Close();
                }
                catch (Exception)
                {
                    controlService.Abort();
                }

                return volume;
            }
            set
            {
                EnsureControlServiceIsAlive();

                PlayerControlServiceClient controlService = new PlayerControlServiceClient(new InstanceContext(this));
                try
                {
                    controlService.SetVolume(value);
                    controlService.Close();
                }
                catch (Exception)
                {
                    controlService.Abort();
                }
            }
        }

        #endregion Properties

        #region Methods

        public static SpotifyRadioTrack ConvertTrack(Track track)
        {
            string albumArt = null;

            if (!string.IsNullOrEmpty(track.Album.CoverID))
            {
                // TODO : Get location of torshify from config, instead of using localhost :o
                albumArt = "http://localhost:1338/torshify/v1/image/id/" + track.Album.CoverID;
            }

            return new SpotifyRadioTrack
            {
                TrackID = track.ID,
                Name = track.Name,
                Artist = track.Album.Artist.Name,
                Album = track.Album.Name,
                AlbumArt = albumArt,
                TotalDuration = TimeSpan.FromMilliseconds(track.Duration)
            };
        }

        public bool CanPlay(IRadioTrack radioTrack)
        {
            return radioTrack is SpotifyRadioTrack;
        }

        public void Stop()
        {
            if (IsPlaying)
            {
                EnsureControlServiceIsAlive();

                PlayerControlServiceClient controlService = new PlayerControlServiceClient(new InstanceContext(this));
                try
                {
                    controlService.TogglePause();
                    controlService.Close();
                }
                catch (Exception)
                {
                    controlService.Abort();
                }

                _currentTrack = null;
                _isPaused = false;
            }
        }

        public void Play()
        {
            if (_currentTrack == null)
                return;

            if (_isPaused)
            {
                EnsureControlServiceIsAlive();

                PlayerControlServiceClient controlService = new PlayerControlServiceClient(new InstanceContext(this));
                try
                {
                    controlService.TogglePause();
                    controlService.Close();
                }
                catch (Exception)
                {
                    controlService.Abort();
                }
            }
            else
            {
                TrackPlayerServiceClient trackPlayer = new TrackPlayerServiceClient();

                try
                {
                    trackPlayer.Play(_currentTrack.TrackID);
                    trackPlayer.Close();
                }
                catch (Exception)
                {
                    trackPlayer.Abort();
                    throw;
                }
            }

            _isPaused = false;
        }

        public void Pause()
        {
            if (IsPlaying)
            {
                EnsureControlServiceIsAlive();

                PlayerControlServiceClient controlService = new PlayerControlServiceClient(new InstanceContext(this));
                try
                {
                    controlService.TogglePause();
                    controlService.Close();
                }
                catch (Exception)
                {
                    controlService.Abort();
                }

                _isPaused = true;
            }
        }

        public void Initialize()
        {
            try
            {
                _controlService = new PlayerControlServiceClient(new InstanceContext(this));
                _controlService.Subscribe();
            }
            catch (Exception)
            {
                _controlService.Abort();

                Thread thread = new Thread(() =>
                {
                    Thread.Sleep(5000);
                    Initialize();
                });
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public void Load(IRadioTrack track)
        {
            SpotifyRadioTrack spotifyTrack = track as SpotifyRadioTrack;

            if (spotifyTrack != null)
            {
                _currentTrack = spotifyTrack;
            }
        }

        void PlayerControlServiceCallback.OnElapsed(double elapsedMs, double totalMs)
        {
            if (IsPlaying)
            {
                var handler = TrackProgress;

                if (handler != null)
                {
                    handler(this, new TrackProgressEventArgs(this, totalMs, elapsedMs));
                }
            }
        }

        void PlayerControlServiceCallback.OnPing()
        {
        }

        void PlayerControlServiceCallback.OnPlayStateChanged(bool isPlaying)
        {
            IsPlaying = isPlaying;

            var handler = IsPlayingChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void PlayerControlServiceCallback.OnTrackChanged(Track track)
        {

        }

        void PlayerControlServiceCallback.OnTrackComplete(Track track)
        {
            var handler = TrackComplete;

            if (handler != null)
            {
                handler(this, new TrackEventArgs(this, ConvertTrack(track)));
            }
        }

        void PlayerControlServiceCallback.OnVolumeChanged(float volume)
        {
        }

        private void EnsureControlServiceIsAlive()
        {
            try
            {
                if (_controlService.State == CommunicationState.Closed || _controlService.State == CommunicationState.Faulted)
                {
                    _controlService.Abort();
                    _controlService = new PlayerControlServiceClient(new InstanceContext(this));
                    _controlService.Subscribe();
                }
            }
            catch (Exception)
            {

            }
        }

        #endregion Methods
    }
}