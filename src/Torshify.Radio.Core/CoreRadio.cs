using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core
{
    [Export(typeof(IRadio))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CoreRadio : NotificationObject, IRadio
    {
        #region Fields

        private readonly ILoadingIndicatorService _loadingIndicatorService;
        private readonly ConcurrentQueue<ITrackStream> _trackStreamQueue;

        private CorePlayer _corePlayer;
        private Track _currentTrack;
        private ITrackStream _currentTrackStream;
        private ConcurrentQueue<Track> _trackQueue;
        private Track _upcomingTrack;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public CoreRadio(CorePlayer corePlayer, ILoadingIndicatorService loadingIndicatorService)
        {
            _trackStreamQueue = new ConcurrentQueue<ITrackStream>();
            _trackQueue = new ConcurrentQueue<Track>();
            _corePlayer = corePlayer;
            _loadingIndicatorService = loadingIndicatorService;
            _corePlayer.Volume = 0.2;
            _corePlayer.TrackComplete += OnTrackComplete;
            _corePlayer.Initialize();
        }

        #endregion Constructors

        #region Events

        public event EventHandler CurrentTrackChanged;

        public event EventHandler CurrentTrackStreamChanged;

        public event EventHandler UpcomingTrackChanged;

        #endregion Events

        #region Properties

        public Track CurrentTrack
        {
            get { return _currentTrack; }
            private set
            {
                if (_currentTrack != value)
                {
                    _currentTrack = value;
                    OnCurrentTrackChanged();
                    RaisePropertyChanged("CurrentTrack");
                }
            }
        }

        public Track UpcomingTrack
        {
            get { return _upcomingTrack; }
            private set
            {
                if (_upcomingTrack != value)
                {
                    _upcomingTrack = value;
                    OnUpcomingTrackChanged();
                    RaisePropertyChanged("UpcomingTrack");
                }
            }
        }

        public IEnumerable<ITrackStream> TrackStreams
        {
            get
            {
                return _trackStreamQueue;
            }
        }

        public bool CanGoToNextTrack
        {
            get
            {
                if (CurrentTrackStream != null)
                {
                    return CurrentTrackStream.SupportsTrackSkipping;
                }

                return false;
            }
        }

        public ITrackStream CurrentTrackStream
        {
            get { return _currentTrackStream; }
            set
            {
                if (_currentTrackStream != value)
                {
                    _currentTrackStream = value;
                    OnCurrentTrackStreamChanged();
                    RaisePropertyChanged("CurrentTrackStream", "CanGoToNextTrack");
                }
            }
        }

        [ImportMany]
        public IEnumerable<Lazy<ITrackPlayer, ITrackPlayerMetadata>> TrackPlayers
        {
            get;
            set;
        }

        [ImportMany]
        public IEnumerable<Lazy<ITrackSource, ITrackSourceMetadata>> TrackSources
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public Track FromLink(TrackLink trackLink)
        {
            var source = TrackSources.FirstOrDefault(s => s.Value.SupportsLink(trackLink));

            if (source != null)
            {
                return source.Value.FromLink(trackLink);
            }

            return null;
        }

        public IEnumerable<TrackContainer> GetAlbumsByArtist(string artist)
        {
            List<TrackContainer> albums = new List<TrackContainer>();

            foreach (var source in TrackSources)
            {
                albums.AddRange(source.Value.GetAlbumsByArtist(artist));
            }

            return albums;
        }

        public IEnumerable<Track> GetTracksByName(string name)
        {
            List<Track> tracks = new List<Track>();

            foreach (var source in TrackSources)
            {
                tracks.AddRange(source.Value.GetTracksByName(name));
            }

            return tracks;
        }

        public void Play(ITrackStream trackStream)
        {
            Task.Factory.StartNew(() =>
                                  {
                                      _loadingIndicatorService.Push();
                                      _trackQueue = new ConcurrentQueue<Track>();
                                      CurrentTrackStream = trackStream;
                                      GetNextBatch();

                                      _corePlayer.Stop();

                                      MoveToNextTrack();
                                      PeekToNextTrack();
                                      _loadingIndicatorService.Pop();
                                  });
        }

        public void Queue(ITrackStream trackStream)
        {
            if (CurrentTrackStream == null)
            {
                Play(trackStream);
            }
            else
            {
                _trackStreamQueue.Enqueue(trackStream);
            }
        }

        public void NextTrack()
        {
            Task.Factory.StartNew(() =>
                                  {
                                      _loadingIndicatorService.Push();
                                      _corePlayer.Stop();

                                      if (_trackQueue.IsEmpty)
                                      {
                                          GetNextBatch();
                                      }

                                      MoveToNextTrack();
                                      PeekToNextTrack();

                                      _loadingIndicatorService.Pop();
                                  });
        }

        public bool SupportsLink(TrackLink trackLink)
        {
            return TrackSources.Any(s => s.Value.SupportsLink(trackLink));
        }

        private void GetNextBatch()
        {
            if (CurrentTrackStream != null && CurrentTrackStream.MoveNext())
            {
                var tracks = CurrentTrackStream.Current;

                foreach (var track in tracks)
                {
                    _trackQueue.Enqueue(track);
                }
            }
            else
            {
                ITrackStream nextTrackStream;
                if (_trackStreamQueue.TryDequeue(out nextTrackStream))
                {
                    CurrentTrackStream = nextTrackStream;
                    GetNextBatch();
                }
                else
                {
                    CurrentTrackStream = null;
                }
            }
        }

        private void PeekToNextTrack()
        {
            Track upcomingTrack;
            if (_trackQueue.TryPeek(out upcomingTrack))
            {
                UpcomingTrack = upcomingTrack;
            }
            else
            {
                UpcomingTrack = null;
            }
        }

        private void MoveToNextTrack()
        {
            Track firstTrack;
            if (_trackQueue.TryDequeue(out firstTrack))
            {
                _corePlayer.Load(firstTrack);
                _corePlayer.Play();

                CurrentTrack = firstTrack;
            }
            else
            {
                CurrentTrack = null;
            }
        }

        private void OnTrackComplete(object sender, TrackEventArgs e)
        {
            Task.Factory.StartNew(() =>
                                  {
                                      _loadingIndicatorService.Push();
                                      if (_trackQueue.IsEmpty)
                                      {
                                          GetNextBatch();
                                      }

                                      MoveToNextTrack();
                                      PeekToNextTrack();
                                      _loadingIndicatorService.Pop();
                                  });
        }

        private void OnCurrentTrackStreamChanged()
        {
            var handler = CurrentTrackStreamChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnCurrentTrackChanged()
        {
            var handler = CurrentTrackChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnUpcomingTrackChanged()
        {
            var handler = UpcomingTrackChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}