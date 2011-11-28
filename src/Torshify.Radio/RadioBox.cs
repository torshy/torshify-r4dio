using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Events;

namespace Torshify.Radio
{
    [Export(typeof(IRadio))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class RadioBox : NotificationObject, IRadio, IPartImportsSatisfiedNotification
    {
        #region Fields

        private IRadioTrack _currentTrack;
        private TimeSpan _currentTrackElapsed;
        private float _currentVolume = 0.5f;
        private ILoggerFacade _logger;
        private readonly IEventAggregator _eventAggregator;
        private RadioNowPlayingViewModel _nowPlayingViewModel;
        private Lazy<IRadioStation, IRadioStationMetadata> _radioStation;
        private RadioStationContext _radioStationContext;
        [ImportMany]
        private IEnumerable<Lazy<IRadioStation, IRadioStationMetadata>> _radioStations = null;
        [Import]
        private IRegionManager _regionManager = null;
        [ImportMany]
        private IEnumerable<IRadioTrackPlayer> _trackPlayers = null;
        [ImportMany]
        private IEnumerable<IRadioTrackSource> _trackSources = null;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public RadioBox(ILoggerFacade logger, IEventAggregator eventAggregator)
        {
            _logger = logger;
            _eventAggregator = eventAggregator;
            _nowPlayingViewModel = new RadioNowPlayingViewModel(this, eventAggregator);
            _nowPlayingViewModel.AtEndOfPlaylist += OnAtEndOfPlaylist;
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsPlayingChanged;

        public event EventHandler<TrackEventArgs> TrackComplete;

        public event EventHandler<TrackProgressEventArgs> TrackProgress;

        #endregion Events

        #region Properties

        public Lazy<IRadioStation, IRadioStationMetadata> CurrentStation
        {
            get { return _radioStation; }
            private set
            {
                _radioStation = value;
                RaisePropertyChanged("CurrentStation");
            }
        }

        public IRadioTrack CurrentTrack
        {
            get { return _currentTrack; }
            private set
            {
                _currentTrack = value;
                RaisePropertyChanged("CurrentTrack", "HasCurrentTrack");
            }
        }

        public TimeSpan CurrentTrackElapsed
        {
            get { return _currentTrackElapsed; }
            private set
            {
                _currentTrackElapsed = value;
                RaisePropertyChanged("CurrentTrackElapsed");
            }
        }

        public bool HasCurrentTrack
        {
            get { return CurrentTrack != null; }
        }

        public bool IsPlaying
        {
            get
            {
                try
                {
                    return _trackPlayers.Any(t => t.IsPlaying);
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, Category.Exception, Priority.Medium);
                }

                return false;
            }
        }

        public IEnumerable<Lazy<IRadioStation, IRadioStationMetadata>> Stations
        {
            get { return _radioStations; }
        }

        public IEnumerable<IRadioTrackPlayer> TrackPlayers
        {
            get { return _trackPlayers; }
        }

        public IEnumerable<IRadioTrackSource> TrackSources
        {
            get { return _trackSources; }
        }

        public float Volume
        {
            get
            {
                return _currentVolume;
            }
            set
            {
                _currentVolume = value;

                var player = GetTrackPlayerForSource(CurrentTrack);

                if (player != null)
                {
                    try
                    {
                        player.Volume = _currentVolume;
                    }
                    catch (Exception e)
                    {
                        _logger.Log(e.Message, Category.Exception, Priority.Medium);
                    }
                }

                RaisePropertyChanged("Volume");
            }
        }

        #endregion Properties

        #region Methods

        public bool CanPlay(IRadioTrack radioTrack)
        {
            try
            {
                return _trackPlayers.Any(t => t.CanPlay(radioTrack));
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, Category.Exception, Priority.Medium);
                return false;
            }
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var radioTrackPlayer in _trackPlayers)
            {
                radioTrackPlayer.TrackComplete += OnRadioStationTrackComplete;
                radioTrackPlayer.TrackProgress += OnRadioStationTrackProgress;
                radioTrackPlayer.IsPlayingChanged += OnRadioStationIsPlayingChanged;

                try
                {
                    radioTrackPlayer.Initialize();
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, Category.Exception, Priority.Medium);
                }
            }

            foreach (var radioTrackSource in _trackSources)
            {
                try
                {
                    radioTrackSource.Initialize();
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, Category.Exception, Priority.Medium);
                }
            }

            foreach (var radioStation in _radioStations)
            {
                try
                {
                    radioStation.Value.Initialize(this);
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, Category.Exception, Priority.Medium);
                }
            }

            _regionManager.Regions.CollectionChanged += OnRegionCollectionChanged;
        }

        object IRadio.GetService(Type serviceType)
        {
            return null;
        }

        void IRadio.TuneIn(Lazy<IRadioStation, IRadioStationMetadata> radioStation)
        {
            if (CurrentStation != null)
            {
                try
                {
                    CurrentStation.Value.OnTunedAway();
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, Category.Exception, Priority.Medium);
                }
            }

            RadioStationContext context = new RadioStationContext(_regionManager, _nowPlayingViewModel);
            context.RemoveCurrentView();
            _radioStationContext = context;
            CurrentStation = radioStation;

            try
            {
                CurrentStation.Value.OnTunedIn(context);
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, Category.Exception, Priority.Medium);
            }
        }

        void IRadioTrackPlayer.Initialize()
        {
        }

        void IRadioTrackPlayer.Load(IRadioTrack track)
        {
            try
            {
                if (CurrentTrack != null)
                {
                    _logger.Log("Stopping " + CurrentTrack.Name, Category.Info, Priority.Low);

                    var currentPlayer = GetTrackPlayerForSource(CurrentTrack);

                    if (currentPlayer != null)
                    {
                        currentPlayer.Stop();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, Category.Exception, Priority.Medium);
            }

            try
            {
                CurrentTrack = track;

                var currentPlayer = GetTrackPlayerForSource(CurrentTrack);

                if (currentPlayer != null)
                {
                    _logger.Log("Loading " + CurrentTrack.Name + " [" + currentPlayer.GetType().Name + "]", Category.Info, Priority.Low);

                    currentPlayer.Load(track);
                }
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, Category.Exception, Priority.Medium);
                _nowPlayingViewModel.MoveToNext();
            }
        }

        void IRadioTrackPlayer.Pause()
        {
            var currentPlayer = GetTrackPlayerForSource(CurrentTrack);

            if (currentPlayer != null)
            {
                try
                {
                    _logger.Log("Pausing" + " [" + currentPlayer.GetType().Name + "]", Category.Info, Priority.Low);

                    currentPlayer.Pause();
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, Category.Exception, Priority.Medium);
                }
            }
        }

        void IRadioTrackPlayer.Play()
        {
            var currentPlayer = GetTrackPlayerForSource(CurrentTrack);

            if (currentPlayer != null)
            {
                try
                {
                    _logger.Log("Playing " + CurrentTrack.Name + " [" + currentPlayer.GetType().Name + "]", Category.Info, Priority.Low);

                    currentPlayer.Play();
                    currentPlayer.Volume = Volume;
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, Category.Exception, Priority.Medium);
                }

                _eventAggregator.GetEvent<TrackChangedEvent>().Publish(CurrentTrack);
            }
        }

        void IRadioTrackPlayer.Stop()
        {
            var currentPlayer = GetTrackPlayerForSource(CurrentTrack);

            if (currentPlayer != null)
            {
                try
                {
                    _logger.Log("Stopping" + " [" + currentPlayer.GetType().Name + "]", Category.Info, Priority.Low);

                    currentPlayer.Stop();
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, Category.Exception, Priority.Medium);
                }
            }
        }

        IEnumerable<IRadioTrack> IRadioTrackSource.GetTracksByAlbum(string artist, string album)
        {
            _logger.Log("GetTracksByAlbum " + artist + " -" + album, Category.Info, Priority.Low);

            ConcurrentBag<IRadioTrack> bag = new ConcurrentBag<IRadioTrack>();

            if (_trackSources != null)
            {
                Parallel.ForEach(_trackSources, trackSource =>
                {
                    try
                    {
                        var result = trackSource.GetTracksByAlbum(artist, album);

                        foreach (var radioTrack in result)
                        {
                            bag.Add(radioTrack);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Log(e.Message, Category.Exception, Priority.Medium);
                    }
                });
            }

            return bag.ToArray();
        }

        IEnumerable<IRadioTrack> IRadioTrackSource.GetTracksByArtist(string artist, int offset, int count)
        {
            _logger.Log("GetTracksByArtist " + artist, Category.Info, Priority.Low);

            ConcurrentBag<IRadioTrack> bag = new ConcurrentBag<IRadioTrack>();

            if (_trackSources != null)
            {
                Parallel.ForEach(_trackSources, trackSource =>
                {
                    try
                    {
                        var result = trackSource.GetTracksByArtist(artist, offset, count);

                        foreach (var radioTrack in result)
                        {
                            bag.Add(radioTrack);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Log(e.Message, Category.Exception, Priority.Medium);
                    }
                });
            }

            return bag.ToArray();
        }

        IEnumerable<IRadioTrack> IRadioTrackSource.GetTracksByName(string name, int offset, int count)
        {
            _logger.Log("GetTracksByName " + name, Category.Info, Priority.Low);

            ConcurrentBag<IRadioTrack> bag = new ConcurrentBag<IRadioTrack>();

            if (_trackSources != null)
            {
                Parallel.ForEach(_trackSources, trackSource =>
                {
                    try
                    {
                        var result = trackSource.GetTracksByName(name, offset, count);

                        foreach (var radioTrack in result)
                        {
                            bag.Add(radioTrack);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Log(e.Message, Category.Exception, Priority.Medium);
                    }
                });
            }

            return bag.ToArray();
        }

        void IRadioTrackSource.Initialize()
        {
        }

        private IRadioTrackPlayer GetTrackPlayerForSource(IRadioTrack radioTrack)
        {
            return _trackPlayers.FirstOrDefault(t => t.CanPlay(radioTrack));
        }

        private void OnAtEndOfPlaylist(object sender, EventArgs e)
        {
            if (_radioStationContext != null)
            {
                try
                {
                    _radioStationContext.GetNextBatch();
                }
                catch (Exception ex)
                {
                    _logger.Log(ex.Message, Category.Exception, Priority.Medium);
                }
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

        private void OnRadioStationIsPlayingChanged(object sender, EventArgs e)
        {
            OnIsPlayingChanged();
            RaisePropertyChanged("IsPlaying");
        }

        private void OnRadioStationTrackComplete(object sender, TrackEventArgs e)
        {
            CurrentTrackElapsed = TimeSpan.Zero;

            var handler = TrackComplete;

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private void OnRadioStationTrackProgress(object sender, TrackProgressEventArgs e)
        {
            CurrentTrackElapsed = TimeSpan.FromMilliseconds(e.ElapsedMilliseconds);

            var handler = TrackProgress;

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private void OnRegionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (IRegion region in e.NewItems)
                {
                    if (region.Name == RegionNames.MainRegion)
                    {
                        ViewData viewData = new ViewData();
                        viewData.Header = "Stations";
                        viewData.View = new Lazy<UIElement>(() => new RadioStationsView { DataContext = new RadioStationsViewModel(this) });
                        region.Add(viewData, RadioStandardViews.Stations);

                        viewData = new ViewData();
                        viewData.Header = "Now playing";
                        viewData.View = new Lazy<UIElement>(() => new RadioNowPlayingView { DataContext = _nowPlayingViewModel });

                        Binding binding = new Binding("HasTracks");
                        binding.Source = _nowPlayingViewModel;
                        binding.Converter = new BooleanToVisibilityConverter();
                        BindingOperations.SetBinding(viewData, ViewData.VisibilityProperty, binding);

                        region.Add(viewData, RadioStandardViews.Tracks);
                    }
                    else if (region.Name == RegionNames.BottomRegion)
                    {
                        region.Add(new RadioControlView { DataContext = new RadioControlViewModel(this) });
                    }
                }
            }
        }

        #endregion Methods
    }
}