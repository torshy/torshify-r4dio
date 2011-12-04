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

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Events;
using Microsoft.Practices.Prism;
using Torshify.Radio.Views.Configuration;

namespace Torshify.Radio
{
    [Export(typeof(IRadio))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class RadioBox : NotificationObject, IRadio, IPartImportsSatisfiedNotification
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private Lazy<IRadioTrackPlayer, IRadioTrackPlayerMetadata> _currentPlayer;
        private RadioTrack _currentTrack;
        private TimeSpan _currentTrackElapsed;
        private float _currentVolume = 0.5f;
        private bool _isBuffering;
        private ILoggerFacade _logger;
        private RadioNowPlayingViewModel _nowPlayingViewModel;
        private Lazy<IRadioStation, IRadioStationMetadata> _radioStation;
        [ImportMany]
        private IEnumerable<Lazy<IRadioStation, IRadioStationMetadata>> _radioStations = null;
        private Random _random;
        private IRegionManager _regionManager = null;
        [ImportMany]
        private IEnumerable<Lazy<IRadioTrackPlayer, IRadioTrackPlayerMetadata>> _trackPlayers = null;
        [ImportMany]
        private IEnumerable<Lazy<IRadioTrackSource, IRadioTrackSourceMetadata>> _trackSources = null;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public RadioBox(ILoggerFacade logger, IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _random = new Random();
            _logger = logger;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _nowPlayingViewModel = new RadioNowPlayingViewModel(this, eventAggregator, regionManager);

            GlobalCommands.OpenSettingsCommand.RegisterCommand(new DelegateCommand(ExecuteOpenSettings));
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsBufferingChanged;

        public event EventHandler IsPlayingChanged;

        public event EventHandler<TrackEventArgs> TrackComplete;

        public event EventHandler<TrackProgressEventArgs> TrackProgress;

        #endregion Events

        #region Properties

        public Lazy<IRadioTrackPlayer, IRadioTrackPlayerMetadata> CurrentPlayer
        {
            get { return _currentPlayer; }
            private set
            {
                if (_currentPlayer != value)
                {
                    _currentPlayer = value;
                    RaisePropertyChanged("CurrentPlayer");
                }
            }
        }

        public Lazy<IRadioStation, IRadioStationMetadata> CurrentStation
        {
            get { return _radioStation; }
            private set
            {
                _radioStation = value;
                RaisePropertyChanged("CurrentStation");
            }
        }

        public RadioTrack CurrentTrack
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

        public bool IsBuffering
        {
            get { return _isBuffering; }
            private set
            {
                _isBuffering = value;
                RaisePropertyChanged("IsBuffering");
            }
        }

        public bool IsPlaying
        {
            get
            {
                try
                {
                    return _trackPlayers.Any(t => t.Value.IsPlaying);
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

        public IEnumerable<Lazy<IRadioTrackPlayer, IRadioTrackPlayerMetadata>> TrackPlayers
        {
            get { return _trackPlayers; }
        }

        public IEnumerable<Lazy<IRadioTrackSource, IRadioTrackSourceMetadata>> TrackSources
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
                        player.Value.Volume = _currentVolume;
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

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var radioTrackPlayer in _trackPlayers)
            {
                radioTrackPlayer.Value.TrackComplete += OnRadioStationTrackComplete;
                radioTrackPlayer.Value.TrackProgress += OnRadioStationTrackProgress;
                radioTrackPlayer.Value.IsPlayingChanged += OnRadioStationIsPlayingChanged;
                radioTrackPlayer.Value.IsBufferingChanged += OnRadioStationIsBufferingChanged;

                try
                {
                    radioTrackPlayer.Value.Initialize();
                }
                catch (Exception e)
                {
                    _logger.Log("Error initializing player " + radioTrackPlayer.Metadata.Name + ". " + e.Message, Category.Exception, Priority.Medium);
                }
            }

            foreach (var radioTrackSource in _trackSources)
            {
                try
                {
                    radioTrackSource.Value.Initialize();
                }
                catch (Exception e)
                {
                    _logger.Log("Error initializing tracksource " + radioTrackSource.Metadata.Name + ". " + e.Message, Category.Exception, Priority.Medium);
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
                    _logger.Log("Error initializing station " + radioStation.Metadata.Name + ". " + e.Message, Category.Exception, Priority.Medium);
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

            CurrentStation = radioStation;

            try
            {
                CurrentStation.Value.OnTunedIn(_nowPlayingViewModel);
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, Category.Exception, Priority.Medium);
            }
        }

        bool IRadioTrackPlayer.CanPlay(RadioTrack radioTrack)
        {
            try
            {
                return _trackPlayers.Any(t => t.Value.CanPlay(radioTrack));
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, Category.Exception, Priority.Medium);
                return false;
            }
        }

        void IRadioTrackPlayer.Initialize()
        {
        }

        void IRadioTrackPlayer.Load(RadioTrack track)
        {
            ((IRadioTrackPlayer)this).Stop();

            CurrentTrack = null;

            var nextPlayer = GetTrackPlayerForSource(track);

            if (nextPlayer != null)
            {
                _logger.Log("Loading " + track.Name + " [" + nextPlayer.Metadata.Name + "]", Category.Info, Priority.Low);

                nextPlayer.Value.Load(track);
                CurrentTrack = track;
            }
        }

        void IRadioTrackPlayer.Pause()
        {
            var currentPlayer = GetTrackPlayerForSource(CurrentTrack);

            if (currentPlayer != null)
            {
                try
                {
                    _logger.Log("Pausing" + " [" + currentPlayer.Metadata.Name + "]", Category.Info, Priority.Low);

                    currentPlayer.Value.Pause();
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
                IsBuffering = false;
                _logger.Log("Playing " + CurrentTrack.Name + " [" + currentPlayer.Metadata.Name + "]", Category.Info, Priority.Low);

                currentPlayer.Value.Play();
                currentPlayer.Value.Volume = Volume;
                CurrentPlayer = currentPlayer;

                _eventAggregator.GetEvent<TrackChangedEvent>().Publish(CurrentTrack);
            }
        }

        void IRadioTrackPlayer.Stop()
        {
            if (CurrentTrack == null)
            {
                return;
            }

            var currentPlayer = GetTrackPlayerForSource(CurrentTrack);

            if (currentPlayer != null)
            {
                try
                {
                    _logger.Log("Stopping" + " [" + currentPlayer.Metadata.Name + "]", Category.Info, Priority.Low);

                    currentPlayer.Value.Stop();
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, Category.Exception, Priority.Medium);
                }
            }

            IsBuffering = false;
        }

        IEnumerable<RadioTrack> IRadioTrackSource.GetTracksByAlbum(string artist, string album)
        {
            _logger.Log("GetTracksByAlbum " + artist + " -" + album, Category.Info, Priority.Low);

            ConcurrentBag<RadioTrack> bag = new ConcurrentBag<RadioTrack>();

            if (_trackSources != null)
            {
                Parallel.ForEach(_trackSources, trackSource =>
                {
                    try
                    {
                        var result = trackSource.Value.GetTracksByAlbum(artist, album);

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

            return bag.OrderBy(x => _random.Next()).ToArray();
        }

        IEnumerable<RadioTrack> IRadioTrackSource.GetTracksByArtist(string artist, int offset, int count)
        {
            ConcurrentBag<RadioTrack> bag = new ConcurrentBag<RadioTrack>();

            if (_trackSources != null)
            {
                Parallel.ForEach(_trackSources, trackSource =>
                {
                    try
                    {
                        var result = trackSource.Value.GetTracksByArtist(artist, offset, count);

                        foreach (var radioTrack in result)
                        {
                            bag.Add(radioTrack);
                        }

                        _logger.Log("GetTracksByArtist " + artist + " @ " + trackSource.Metadata.Name + " = " + result.Count(), Category.Info, Priority.Low);
                    }
                    catch (Exception e)
                    {
                        _logger.Log(e.ToString(), Category.Exception, Priority.Medium);
                    }
                });
            }

            return bag.OrderBy(x => _random.Next()).ToArray();
        }

        IEnumerable<RadioTrack> IRadioTrackSource.GetTracksByName(string name, int offset, int count)
        {
            _logger.Log("GetTracksByName " + name, Category.Info, Priority.Low);

            ConcurrentBag<RadioTrack> bag = new ConcurrentBag<RadioTrack>();

            if (_trackSources != null)
            {
                Parallel.ForEach(_trackSources, trackSource =>
                {
                    try
                    {
                        var result = trackSource.Value.GetTracksByName(name, offset, count);

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

            return bag.OrderBy(x => _random.Next()).ToArray();
        }

        void IRadioTrackSource.Initialize()
        {
        }

        private void ExecuteOpenSettings()
        {
            var region = _regionManager.Regions[RegionNames.MainRegion];
            var settingsView = region.GetView("Settings");

            if (settingsView != null)
            {
                region.Activate(settingsView);
            }
            else
            {
                ViewData viewData = new ViewData();
                viewData.Header = "Settings";
                viewData.View = new Lazy<UIElement>(() => ServiceLocator.Current.TryResolve<ConfigurationView>());

                region.Add(viewData, "Settings");
                region.Activate(viewData);
            }
        }

        private Lazy<IRadioTrackPlayer, IRadioTrackPlayerMetadata> GetTrackPlayerForSource(RadioTrack radioTrack)
        {
            return _trackPlayers.FirstOrDefault(t => t.Value.CanPlay(radioTrack));
        }

        private void OnIsPlayingChanged()
        {
            var handler = IsPlayingChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnRadioStationIsBufferingChanged(object sender, EventArgs e)
        {
            IRadioTrackPlayer player = (IRadioTrackPlayer)sender;
            IsBuffering = player.IsBuffering;
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