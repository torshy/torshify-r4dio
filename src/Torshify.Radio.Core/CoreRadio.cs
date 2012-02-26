using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;

using Raven.Client;

using Torshify.Radio.Core.Models;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core
{
    [Export(typeof(IRadio))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CoreRadio : NotificationObject, IRadio
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private readonly IDocumentStore _documentStore;
        private readonly ILoadingIndicatorService _loadingIndicatorService;
        private readonly ILoggerFacade _logger;
        private readonly IToastService _toastService;
        private readonly ConcurrentQueue<ITrackStream> _trackStreamQueue;

        private ITrackPlayer _corePlayer;
        private Track _currentTrack;
        private ITrackStream _currentTrackStream;
        private ConcurrentQueue<Track> _trackQueue;
        private ObservableCollection<Track> _trackQueuePublic;
        private ObservableCollection<ITrackStream> _trackStreamQueuePublic;
        private Track _upcomingTrack;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public CoreRadio(
            [Import("CorePlayer")]ITrackPlayer corePlayer,
            IDocumentStore documentStore,
            ILoadingIndicatorService loadingIndicatorService,
            IToastService toastService,
            ILoggerFacade logger,
            Dispatcher dispatcher)
        {
            _trackQueuePublic = new ObservableCollection<Track>();
            _trackStreamQueuePublic = new ObservableCollection<ITrackStream>();
            _trackStreamQueue = new ConcurrentQueue<ITrackStream>();
            _trackQueue = new ConcurrentQueue<Track>();
            _corePlayer = corePlayer;
            _documentStore = documentStore;
            _loadingIndicatorService = loadingIndicatorService;
            _toastService = toastService;
            _logger = logger;
            _dispatcher = dispatcher;
            _corePlayer.Volume = 0.2;
            _corePlayer.TrackComplete += OnTrackComplete;
            _corePlayer.Initialize();
        }

        #endregion Constructors

        #region Events

        public event EventHandler<TrackChangedEventArgs> CurrentTrackChanged;

        public event EventHandler CurrentTrackStreamChanged;

        public event EventHandler TrackStreamQueued;

        public event EventHandler UpcomingTrackChanged;

        #endregion Events

        #region Properties

        public Track CurrentTrack
        {
            get
            {
                return _currentTrack;
            }
            private set
            {
                if (_currentTrack != value)
                {
                    var previousTrack = _currentTrack;
                    _currentTrack = value;
                    RaisePropertyChanged("CurrentTrack");
                    OnCurrentTrackChanged(new TrackChangedEventArgs(previousTrack, _currentTrack));
                }
            }
        }

        public Track UpcomingTrack
        {
            get
            {
                return _upcomingTrack;
            }
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

        public IEnumerable<Track> UpcomingTracks
        {
            get
            {
                return _trackQueuePublic;
            }
        }

        public IEnumerable<ITrackStream> TrackStreams
        {
            get
            {
                return _trackStreamQueuePublic;
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

        public bool CanGoToNextTrackStream
        {
            get
            {
                return !_trackStreamQueue.IsEmpty;
            }
        }

        public ITrackStream CurrentTrackStream
        {
            get
            {
                return _currentTrackStream;
            }
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
            _logger.Log("Getting albums by artist [" + artist + "]", Category.Info, Priority.Low);

            List<TrackContainer> albums = new List<TrackContainer>();

            foreach (var source in GetTrackSources())
            {
                albums.AddRange(source.Value.GetAlbumsByArtist(artist));
            }

            return albums;
        }

        public IEnumerable<Track> GetTracksByName(string name)
        {
            _logger.Log("Getting tracks by name [" + name + "]", Category.Info, Priority.Low);

            List<Track> tracks = new List<Track>();

            foreach (var source in GetTrackSources())
            {
                tracks.AddRange(source.Value.GetTracksByName(name));
            }

            return tracks;
        }

        public void Play(ITrackStream trackStream)
        {
            _trackQueue = new ConcurrentQueue<Track>();
            _dispatcher.BeginInvoke(new Action(_trackQueuePublic.Clear));

            CurrentTrackStream = trackStream;

            Task
                .Factory
                .StartNew(() =>
                {
                    using (_loadingIndicatorService.EnterLoadingBlock())
                    {
                        GetNextBatch();

                        _corePlayer.Stop();

                        MoveToNextTrack();
                        PeekToNextTrack();
                    }
                })
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        task.Exception.Handle(e =>
                        {
                            _toastService.Show("Error while playing track.");
                            _logger.Log(e.ToString(), Category.Exception, Priority.High);
                            return true;
                        });
                    }
                });
        }

        public void Queue(ITrackStream trackStream)
        {
            if (CurrentTrackStream == null)
            {
                _logger.Log("No track stream is already playing. Starting " + trackStream.Description, Category.Debug, Priority.Low);
                Play(trackStream);
            }
            else
            {
                _trackStreamQueue.Enqueue(trackStream);
                _logger.Log("Queued track stream " + trackStream.Description, Category.Debug, Priority.Low);
                _dispatcher.BeginInvoke(new Action<ITrackStream>(q=>
                {
                    _trackStreamQueuePublic.Add(q);
                    OnTrackStreamQueued();
                }), trackStream);
            }

            RaisePropertyChanged("CanGoToNextTrackStream");
        }

        public void NextTrack()
        {
            Task
                .Factory
                .StartNew(() =>
                {
                    using (_loadingIndicatorService.EnterLoadingBlock())
                    {
                        _corePlayer.Stop();

                        if (_trackQueue.IsEmpty)
                        {
                            _logger.Log("Current track stream is empty. Trying to move on", Category.Debug, Priority.Low);
                            GetNextBatch();
                        }

                        MoveToNextTrack();
                        PeekToNextTrack();
                    }
                })
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        task.Exception.Handle(e =>
                        {
                            _toastService.Show("Error while playing track.");
                            _logger.Log(e.ToString(), Category.Exception, Priority.High);
                            return true;
                        });
                    }
                });
        }

        public void NextTrackStream()
        {
            if (CanGoToNextTrackStream)
            {
                Task
                    .Factory
                    .StartNew(() =>
                    {
                        _trackQueue = new ConcurrentQueue<Track>();
                        _dispatcher.BeginInvoke(new Action(_trackQueuePublic.Clear));
                        UpcomingTrack = null;

                        ITrackStream nextTrackStream;
                        if (_trackStreamQueue.TryDequeue(out nextTrackStream))
                        {
                            _logger.Log("Changing current track stream to " + nextTrackStream.Description, Category.Debug, Priority.Low);
                            _dispatcher.BeginInvoke(new Func<ITrackStream, bool>(_trackStreamQueuePublic.Remove), nextTrackStream);

                            CurrentTrackStream = nextTrackStream;
                            GetNextBatch();
                            MoveToNextTrack();
                            PeekToNextTrack();
                        }
                    })
                    .ContinueWith(task =>
                    {
                        if (task.Exception != null)
                        {
                            task.Exception.Handle(e =>
                            {
                                _toastService.Show("Error while playing track.");
                                _logger.Log(e.ToString(), Category.Exception, Priority.High);
                                return true;
                            });
                        }

                        RaisePropertyChanged("CanGoToNextTrackStream");
                    });
            }
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
                    _logger.Log("Adding " + track.Name + " - " + track.Artist + " to playlist", Category.Debug, Priority.Low);
                    _trackQueue.Enqueue(track);
                    _dispatcher.BeginInvoke(new Action<Track>(_trackQueuePublic.Add), track);
                }
            }
            else
            {
                ITrackStream nextTrackStream;
                if (_trackStreamQueue.TryDequeue(out nextTrackStream))
                {
                    _logger.Log("Changing current track stream to " + nextTrackStream.Description, Category.Debug, Priority.Low);
                    _dispatcher.BeginInvoke(new Func<ITrackStream, bool>(_trackStreamQueuePublic.Remove), nextTrackStream);
                    CurrentTrackStream = nextTrackStream;
                    GetNextBatch();
                }
                else
                {
                    _logger.Log("No more track streams to play", Category.Debug, Priority.Low);
                    CurrentTrackStream = null;
                }

                RaisePropertyChanged("CanGoToNextTrackStream");
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
                _dispatcher.BeginInvoke(new Func<Track, bool>(_trackQueuePublic.Remove), firstTrack);
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
            Task
                .Factory
                .StartNew(() =>
                {
                    using (_loadingIndicatorService.EnterLoadingBlock())
                    {
                        if (_trackQueue.IsEmpty)
                        {
                            GetNextBatch();
                        }

                        MoveToNextTrack();
                        PeekToNextTrack();
                    }
                })
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        task.Exception.Handle(ex =>
                        {
                            _toastService.Show("Error while playing track.");
                            _logger.Log(ex.ToString(), Category.Exception, Priority.High);
                            return true;
                        });
                    }
                });
        }

        private IEnumerable<Lazy<ITrackSource, ITrackSourceMetadata>> GetTrackSources()
        {
            try
            {
                List<Lazy<ITrackSource, ITrackSourceMetadata>> sources = new List<Lazy<ITrackSource, ITrackSourceMetadata>>();
                using(var session = _documentStore.OpenSession())
                {
                    var appSettings = session.Query<ApplicationSettings>().FirstOrDefault();

                    if (appSettings != null)
                    {
                        if (appSettings.TrackSources.Any())
                        {
                            foreach (var trackSource in appSettings.TrackSources)
                            {
                                var source = TrackSources.FirstOrDefault(s => s.Metadata.Name == trackSource.Name && !trackSource.Disabled);

                                if (source != null)
                                {
                                    sources.Add(source);
                                }
                            }

                            var except = TrackSources.Except(sources);
                            sources.AddRange(except);
                            return sources;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                _logger.Log(e.ToString(), Category.Exception, Priority.Medium);
            }

            return TrackSources;
        }

        private void OnCurrentTrackStreamChanged()
        {
            var handler = CurrentTrackStreamChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnCurrentTrackChanged(TrackChangedEventArgs e)
        {
            var handler = CurrentTrackChanged;

            if (handler != null)
            {
                handler(this, e);
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

        private void OnTrackStreamQueued()
        {
            var handler = TrackStreamQueued;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}