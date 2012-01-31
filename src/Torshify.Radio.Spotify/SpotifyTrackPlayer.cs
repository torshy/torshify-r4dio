using System;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

using Torshify.Radio.Framework;
using Torshify.Radio.Spotify.LoginService;
using Torshify.Radio.Spotify.PlayerControlService;
using Torshify.Radio.Spotify.TrackPlayerService;

namespace Torshify.Radio.Spotify
{
    [TrackPlayerMetadata(Name = "Spotify", IconUri = "pack://application:,,,/Torshify.Radio.Spotify;component;Resources/Spotify_Logo.png")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SpotifyRadioTrackPlayer : ITrackPlayer, PlayerControlServiceCallback, LoginServiceCallback
    {
        #region Fields

        private PlayerControlServiceClient _controlService;
        private SpotifyTrack _currentTrack;
        private bool _isPaused;
        private bool _isPlaying;
        private LoginServiceClient _loginClient;

        #endregion Fields

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
            get { return false; }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
            }
        }

        public bool IsMuted
        {
            get;
            set;
        }

        public double Volume
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
                    controlService.SetVolume((float)value);
                    controlService.Close();
                }
                catch (Exception)
                {
                    controlService.Abort();
                }
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
            get { return true; }
        }

        #endregion Properties

        #region Methods

        public static SpotifyTrack ConvertTrack(Origo.Contracts.V1.Track track)
        {
            string albumArt = null;

            if (!string.IsNullOrEmpty(track.Album.CoverID))
            {
                // TODO : Get location of torshify from config, instead of using localhost :o
                albumArt = "http://localhost:1338/torshify/v1/image/id/" + track.Album.CoverID;
            }

            return new SpotifyTrack
            {
                TrackId = track.ID,
                Name = track.Name,
                Artist = track.Album.Artist.Name,
                Album = track.Album.Name,
                AlbumArt = albumArt,
                TotalDuration = TimeSpan.FromMilliseconds(track.Duration)
            };
        }

        public bool CanPlay(Track radioTrack)
        {
            return radioTrack is SpotifyTrack;
        }

        public void Initialize()
        {
            Task.Factory.StartNew(() =>
                                  {
                                      try
                                      {
                                          _loginClient = new LoginServiceClient(new InstanceContext(this));
                                          _loginClient.Subscribe();

                                          if (!_loginClient.IsLoggedIn())
                                          {
                                              if (!string.IsNullOrEmpty(_loginClient.GetRememberedUser()))
                                              {
                                                  _loginClient.Relogin();
                                              }
                                          }
                                          else
                                          {
                                              SubscribeToControlEvents();
                                          }
                                      }
                                      catch
                                      {
                                          _loginClient.Abort();
                                          Thread.Sleep(2000);
                                          Initialize();
                                      }
                                  });
        }

        public void Load(Track track)
        {
            SpotifyTrack spotifyTrack = track as SpotifyTrack;

            if (spotifyTrack != null)
            {
                _currentTrack = spotifyTrack;
            }
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
                    trackPlayer.Play(_currentTrack.TrackId);
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

        void LoginServiceCallback.OnLoggedIn()
        {
            SubscribeToControlEvents();
        }

        void LoginServiceCallback.OnLoginError(string message)
        {
            Console.WriteLine(message);
        }

        void LoginServiceCallback.OnLoggedOut()
        {
        }

        void LoginServiceCallback.OnPing()
        {
        }

        void PlayerControlServiceCallback.OnElapsed(double elapsedMs, double totalMs)
        {
            if (IsPlaying)
            {
                var handler = TrackProgress;

                if (handler != null)
                {
                    handler(this, new TrackProgressEventArgs(totalMs, elapsedMs));
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

        void PlayerControlServiceCallback.OnTrackChanged(Origo.Contracts.V1.Track track)
        {
        }

        void PlayerControlServiceCallback.OnTrackComplete(Origo.Contracts.V1.Track track)
        {
            var handler = TrackComplete;

            if (handler != null)
            {
                handler(this, new TrackEventArgs(ConvertTrack(track)));
            }
        }

        void PlayerControlServiceCallback.OnVolumeChanged(float volume)
        {
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

        public void Dispose()
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

        private void SubscribeToControlEvents()
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

        #endregion Methods
    }
}