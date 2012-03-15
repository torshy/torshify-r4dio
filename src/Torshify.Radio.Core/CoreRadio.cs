using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
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
        private CancellationTokenSource _currentTokenSource;

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
                var result = source.Value.GetAlbumsByArtist(artist).ToArray();

                _logger.Log("[" + source.Metadata.Name + "] " + result.Length + " results found", Category.Info, Priority.Low);

                albums.AddRange(result);
            }

            return albums;
        }

        public IEnumerable<Track> GetTracksByName(string name)
        {
            _logger.Log("Getting tracks by name [" + name + "]", Category.Info, Priority.Low);

            List<Track> tracks = new List<Track>();

            foreach (var source in GetTrackSources())
            {
                var result = source.Value.GetTracksByName(name).ToArray();

                _logger.Log("[" + source.Metadata.Name + "] " + result.Length + " results found", Category.Info, Priority.Low);

                tracks.AddRange(result);
            }

            return tracks;
        }

        public void Play(ITrackStream trackStream)
        {
            _trackQueue = new ConcurrentQueue<Track>();
            _dispatcher.BeginInvoke(new Action(_trackQueuePublic.Clear));

            CurrentTrackStream = trackStream;

            try
            {
                if (_currentTokenSource != null && !_currentTokenSource.IsCancellationRequested)
                {
                    _currentTokenSource.Cancel();
                }
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), Category.Exception, Priority.Low);
            }

            _currentTokenSource = new CancellationTokenSource();
            var tct = _currentTokenSource.Token;

            Task
                .Factory
                .StartNew(() =>
                {
                    using (_loadingIndicatorService.EnterLoadingBlock())
                    {
                        GetNextBatch(tct);

                        _corePlayer.Stop();

                        while (!MoveToNextTrack())
                        {
                            if (!GetNextBatch(tct))
                            {
                                break;
                            }
                        }

                        PeekToNextTrack();
                    }
                }, tct)
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
                _dispatcher.BeginInvoke(new Action<ITrackStream>(q =>
                {
                    _trackStreamQueuePublic.Add(q);
                    OnTrackStreamQueued();
                }), trackStream);
            }

            RaisePropertyChanged("CanGoToNextTrackStream");
        }

        public void NextTrack()
        {
            _currentTokenSource = new CancellationTokenSource();
            var tct = _currentTokenSource.Token;

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
                            GetNextBatch(tct);
                        }

                        while (!MoveToNextTrack())
                        {
                            if (!GetNextBatch(tct))
                            {
                                break;
                            }
                        }

                        PeekToNextTrack();
                    }
                }, tct)
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
                _currentTokenSource = new CancellationTokenSource();
                var tct = _currentTokenSource.Token;

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
                            GetNextBatch(tct);

                            while (!MoveToNextTrack())
                            {
                                if (!GetNextBatch(tct))
                                {
                                    break;
                                }
                            }

                            PeekToNextTrack();
                        }
                    }, tct)
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

        private bool GetNextBatch(CancellationToken token)
        {
            if (CurrentTrackStream != null && CurrentTrackStream.MoveNext(token))
            {
                var tracks = CurrentTrackStream.Current;

                foreach (var track in tracks)
                {
                    _logger.Log("Adding " + track.Name + " - " + track.Artist + " to playlist", Category.Debug, Priority.Low);
                    _trackQueue.Enqueue(track);
                    _dispatcher.BeginInvoke(new Action<Track>(_trackQueuePublic.Add), track);
                }

                RaisePropertyChanged("CanGoToNextTrackStream");

                return true;
            }

            ITrackStream nextTrackStream;
            if (_trackStreamQueue.TryDequeue(out nextTrackStream))
            {
                _logger.Log("Changing current track stream to " + nextTrackStream.Description, Category.Debug, Priority.Low);
                _dispatcher.BeginInvoke(new Func<ITrackStream, bool>(_trackStreamQueuePublic.Remove), nextTrackStream);
                CurrentTrackStream = nextTrackStream;
                RaisePropertyChanged("CanGoToNextTrackStream");
                return GetNextBatch(token);
            }

            _logger.Log("No more track streams to play", Category.Debug, Priority.Low);
            CurrentTrackStream = null;
            RaisePropertyChanged("CanGoToNextTrackStream");

            return false;
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

        private bool MoveToNextTrack()
        {
            Track firstTrack;
            if (_trackQueue.TryDequeue(out firstTrack))
            {
                try
                {
                    _dispatcher.BeginInvoke(new Func<Track, bool>(_trackQueuePublic.Remove), firstTrack);
                    _corePlayer.Load(firstTrack);
                    _corePlayer.Play();

                    CurrentTrack = firstTrack;
                    return true;
                }
                catch (Exception e)
                {
                    _logger.Log(e.ToString(), Category.Exception, Priority.Low);
                    return MoveToNextTrack();
                }
            }

            CurrentTrack = null;
            return false;
        }

        private void OnTrackComplete(object sender, TrackEventArgs e)
        {
            _currentTokenSource = new CancellationTokenSource();
            var tct = _currentTokenSource.Token;

            Task
                .Factory
                .StartNew(() =>
                {
                    using (_loadingIndicatorService.EnterLoadingBlock())
                    {
                        if (_trackQueue.IsEmpty)
                        {
                            GetNextBatch(tct);
                        }

                        while (!MoveToNextTrack())
                        {
                            if (!GetNextBatch(tct))
                            {
                                break;
                            }
                        }

                        PeekToNextTrack();
                    }
                }, tct)
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
                var sources = new List<Lazy<ITrackSource, ITrackSourceMetadata>>();
                using (var session = _documentStore.OpenSession())
                {
                    var appSettings = session.Query<ApplicationSettings>().FirstOrDefault();

                    if (appSettings != null)
                    {
                        if (appSettings.TrackSources.Any())
                        {
                            foreach (var sourceConfig in appSettings.TrackSources)
                            {
                                if (sourceConfig.Disabled)
                                {
                                    continue;
                                }

                                var source =
                                    TrackSources.FirstOrDefault(
                                        ts =>
                                        ts.Metadata.Name.Equals(sourceConfig.Name,
                                                                StringComparison.InvariantCultureIgnoreCase));

                                if (source != null)
                                {
                                    sources.Insert(sourceConfig.Index, source);
                                }
                            }

                            // TODO : Need to handle a situation where there are new track sources which doesn't exist in db
                            //var configuredNames = appSettings.TrackSources.Select(tsc => tsc.Name);
                            //var availableNames = TrackSources.Select(ts => ts.Metadata.Name);
                            //var mismatch = availableNames.Except(configuredNames);

                            //if (mismatch.Any())
                            //{

                            //}

                            return sources;
                        }
                    }
                }
            }
            catch (Exception e)
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