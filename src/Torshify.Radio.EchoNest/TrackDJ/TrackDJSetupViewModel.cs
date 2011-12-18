using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using EchoNest;
using EchoNest.Song;

using GMap.NET;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.EchoNest.Mood;
using Torshify.Radio.EchoNest.Style;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Common;

namespace Torshify.Radio.EchoNest.TrackDJ
{
    [Export(typeof(TrackDJSetupViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TrackDJSetupViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IRadio _radio;

        private bool _artistFamiliarityEnabled;
        private bool _artistHotttnessEnabled;
        private double? _artistMaxFamiliarity;
        private double? _artistMaxHotttness;
        private double? _artistMinFamiliarity;
        private double? _artistMinHotttness;
        private double? _maxDanceability;
        private double? _maxEnergy;
        private double? _minDanceability;
        private double? _minEnergy;
        private IRegionNavigationService _navigationService;
        private List<TermModel> _selectedMoods;
        private List<TermModel> _selectedStyles;
        private double? _songMaxHotttness;
        private double? _songMinHotttness;
        private bool _danceabliltyEnabled;
        private bool _energyEnabled;
        private bool _songHotttnessEnabled;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public TrackDJSetupViewModel(IRadio radio)
        {
            _radio = radio;
            _selectedMoods = new List<TermModel>();
            _selectedStyles = new List<TermModel>();

            ArtistHotttnessEnabled = true;
            ArtistMaxHotttness = 1.0;
            ArtistMinHotttness = 0.0;
            ArtistFamiliarityEnabled = true;
            ArtistMaxFamiliarity = 1.0;
            ArtistMinFamiliarity = 0.0;
            SongHotttnessEnabled = true;
            SongMaxHotttness = 1.0;
            SongMinHotttness = 0.0;
            DanceabliltyEnabled = true;
            MaxDanceablilty = 1.0;
            MinDanceablilty = 0.0;
            EnergyEnabled = true;
            MaxEnergy = 1.0;
            MinEnergy = 0.0;

            AvailableMoods = new ObservableCollection<TermModel>();
            AvailableStyles = new ObservableCollection<TermModel>();

            FindTracksCommand = new DelegateCommand(ExecuteFindTracks);
            CreatePlaylistCommand = new DelegateCommand(ExecuteCreatePlaylist);
            StyleSelectionChanged = new DelegateCommand<IEnumerable>(ExecuteStyleSelectionChanged);
            MoodSelectionChanged = new DelegateCommand<IEnumerable>(ExecuteMoodSelectionChanged);

            GetAvailableStylesAndMoods();
        }

        #endregion Constructors

        #region Properties

        public bool ArtistFamiliarityEnabled
        {
            get { return _artistFamiliarityEnabled; }
            set
            {
                if (_artistFamiliarityEnabled != value)
                {
                    _artistFamiliarityEnabled = value;
                    RaisePropertyChanged("ArtistFamiliarityEnabled");
                }
            }
        }

        public bool ArtistHotttnessEnabled
        {
            get { return _artistHotttnessEnabled; }
            set
            {
                if (_artistHotttnessEnabled != value)
                {
                    _artistHotttnessEnabled = value;
                    RaisePropertyChanged("ArtistHotttnessEnabled");
                }
            }
        }

        public double? ArtistMaxFamiliarity
        {
            get { return _artistMaxFamiliarity; }
            set
            {
                if (_artistMaxFamiliarity != value)
                {
                    _artistMaxFamiliarity = value;
                    RaisePropertyChanged("ArtistMaxFamiliarity");
                }
            }
        }

        public double? ArtistMaxHotttness
        {
            get { return _songMaxHotttness; }
            set
            {
                if (_artistMaxHotttness != value)
                {
                    _artistMaxHotttness = value;
                    RaisePropertyChanged("ArtistMaxHotttness");
                }
            }
        }

        public double? ArtistMinFamiliarity
        {
            get { return _artistMinFamiliarity; }
            set
            {
                if (_artistMinFamiliarity != value)
                {
                    _artistMinFamiliarity = value;
                    RaisePropertyChanged("ArtistMinFamiliarity");
                }
            }
        }

        public double? ArtistMinHotttness
        {
            get { return _songMinHotttness; }
            set
            {
                if (_artistMinHotttness != value)
                {
                    _artistMinHotttness = value;
                    RaisePropertyChanged("ArtistMinHotttness");
                }
            }
        }

        public ObservableCollection<TermModel> AvailableMoods
        {
            get;
            private set;
        }

        public ObservableCollection<TermModel> AvailableStyles
        {
            get;
            private set;
        }

        public ICommand CreatePlaylistCommand
        {
            get;
            private set;
        }

        public ICommand FindTracksCommand
        {
            get;
            private set;
        }

        public Func<RectLatLng> GetViewArea
        {
            get;
            set;
        }

        public bool DanceabliltyEnabled
        {
            get { return _danceabliltyEnabled; }
            set
            {
                if (_danceabliltyEnabled != value)
                {
                    _danceabliltyEnabled = value;
                    RaisePropertyChanged("DanceabliltyEnabled");
                }
            }
        }

        public double? MaxDanceablilty
        {
            get { return _maxDanceability; }
            set
            {
                if (_maxDanceability != value)
                {
                    _maxDanceability = value;
                    RaisePropertyChanged("MaxDanceablilty");
                }
            }
        }

        public bool EnergyEnabled
        {
            get { return _energyEnabled; }
            set
            {
                if (_energyEnabled != value)
                {
                    _energyEnabled = value;
                    RaisePropertyChanged("EnergyEnabled");
                }
            }
        }

        public double? MaxEnergy
        {
            get { return _maxEnergy; }
            set
            {
                if (_maxEnergy != value)
                {
                    _maxEnergy = value;
                    RaisePropertyChanged("MaxEnergy");
                }
            }
        }

        public double? MinDanceablilty
        {
            get { return _minDanceability; }
            set
            {
                if (_minDanceability != value)
                {
                    _minDanceability = value;
                    RaisePropertyChanged("MinDanceablilty");
                }
            }
        }

        public double? MinEnergy
        {
            get { return _minEnergy; }
            set
            {
                if (_minEnergy != value)
                {
                    _minEnergy = value;
                    RaisePropertyChanged("MinEnergy");
                }
            }
        }

        public DelegateCommand<IEnumerable> MoodSelectionChanged
        {
            get;
            private set;
        }

        public List<TermModel> SelectedMoods
        {
            get { return _selectedMoods; }
        }

        public List<TermModel> SelectedStyles
        {
            get { return _selectedStyles; }
        }

        public bool SongHotttnessEnabled
        {
            get { return _songHotttnessEnabled; }
            set
            {
                if (_songHotttnessEnabled != value)
                {
                    _songHotttnessEnabled = value;
                    RaisePropertyChanged("SongHotttnessEnabled");
                }
            }
        }

        public double? SongMaxHotttness
        {
            get { return _songMaxHotttness; }
            set
            {
                if (_songMaxHotttness != value)
                {
                    _songMaxHotttness = value;
                    RaisePropertyChanged("SongMaxHotttness");
                }
            }
        }

        public double? SongMinHotttness
        {
            get { return _songMinHotttness; }
            set
            {
                if (_songMinHotttness != value)
                {
                    _songMinHotttness = value;
                    RaisePropertyChanged("SongMinHotttness");
                }
            }
        }

        public DelegateCommand<IEnumerable> StyleSelectionChanged
        {
            get;
            private set;
        }

        public Func<RectLatLng> GetSelectedArea
        {
            get; 
            set;
        }

        #endregion Properties

        #region Methods

        public SearchArgument CreateSearchArgument()
        {
            var arg = new SearchArgument();

            if (ArtistFamiliarityEnabled)
            {
                arg.ArtistMaxFamiliarity = ArtistMaxFamiliarity;
                arg.ArtistMinFamiliarity = ArtistMinFamiliarity;
            }

            if (ArtistHotttnessEnabled)
            {
                arg.ArtistMaxHotttnesss = ArtistMaxHotttness;
                arg.ArtistMinHotttnesss = ArtistMinHotttness;
            }

            if (SongHotttnessEnabled)
            {
                arg.SongMaxHotttnesss = SongMaxHotttness;
                arg.SongMinHotttnesss = SongMinHotttness;
            }

            if (DanceabliltyEnabled)
            {
                arg.MaxDanceability = MaxDanceablilty;
                arg.MinDanceability = MinDanceablilty;
            }

            if (EnergyEnabled)
            {
                arg.MaxEnergy = MaxEnergy;
                arg.MinEnergy = MinEnergy;
            }

            RectLatLng viewArea = GetViewArea();
            RectLatLng selectedArea = GetSelectedArea();
            RectLatLng currentArea = selectedArea;

            if (currentArea.IsEmpty)
            {
                currentArea = viewArea;
            }

            arg.MaxLatitude = currentArea.Top;
            arg.MinLatitude = currentArea.Bottom;
            arg.MaxLongitude = currentArea.Right;
            arg.MinLongitude = currentArea.Left;

            AddTerms(arg.Moods, SelectedMoods);
            AddTerms(arg.Styles, SelectedStyles);

            return arg;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
        }

        private void AddTerms(TermList termList, IEnumerable<TermModel> selectedTerms)
        {
            foreach (var termModel in selectedTerms)
            {
                var term = termList.Add(termModel.Name);

                if (!DoubleUtilities.AreClose(termModel.Boost, 1.0))
                {
                    term.Boost(termModel.Boost);
                }

                if (termModel.Require)
                {
                    term.Require();
                }

                if (termModel.Ban)
                {
                    term.Ban();
                }
            }
        }

        private void ExecuteCreatePlaylist()
        {
            TrackDJSongEnumerator enumerator = new TrackDJSongEnumerator();
            enumerator.Initialize(CreateSearchArgument(), _radio);
            _radio.CurrentContext.SetTrackProvider(new TrackProvider(enumerator.DoIt));
        }

        private void ExecuteFindTracks()
        {
            _navigationService.Region.Context = this;
            _navigationService.RequestNavigate(new Uri(typeof (TrackDJResultsView).FullName, UriKind.Relative));
        }

        private void ExecuteMoodSelectionChanged(IEnumerable enumerable)
        {
            _selectedMoods = new List<TermModel>(enumerable.OfType<TermModel>());
        }

        private void ExecuteStyleSelectionChanged(IEnumerable enumerable)
        {
            _selectedStyles = new List<TermModel>(enumerable.OfType<TermModel>());
        }

        private void GetAvailableStylesAndMoods()
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task<IEnumerable<TermModel>>.Factory
                .StartNew(MoodRadioStationViewModel.GetAvailableMoods)
                .ContinueWith(t =>
                                  {
                                      AvailableMoods.Clear();

                                      foreach (var termModel in t.Result)
                                      {
                                          AvailableMoods.Add(termModel);
                                      }
                                  }, ui);

            Task<IEnumerable<TermModel>>.Factory
                .StartNew(StyleRadioStationViewModel.GetAvailableStyles)
                .ContinueWith(t =>
                                  {
                                      AvailableStyles.Clear();

                                      foreach (var termModel in t.Result)
                                      {
                                          AvailableStyles.Add(termModel);
                                      }
                                  }, ui);
        }

        #endregion Methods
    }
}