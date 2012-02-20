using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

using EchoNest;
using EchoNest.Playlist;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.EchoNest.Services;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.EchoNest.Views.LoveHate
{
    [Export]
    [RegionMemberLifetime(KeepAlive = false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoveHateViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private double? _currentTrackRating;
        private bool? _dislikeCurrentTrack;
        private bool _hasTrack;
        private bool? _likeCurrentTrack;
        private LoveHateTrackStream _loveHateTrackStream;

        #endregion Fields

        #region Constructors

        public LoveHateViewModel()
        {
            _dislikeCurrentTrack = false;
            _likeCurrentTrack = false;
            SkipCurrentTrackCommand = new StaticCommand(ExecuteSkipCurrentTrack);
        }

        #endregion Constructors

        #region Properties

        public StaticCommand SkipCurrentTrackCommand
        {
            get;
            private set;
        }

        [Import]
        public ISuggestArtistsService SuggestArtistsService
        {
            get;
            set;
        }

        [Import]
        public IToastService ToastService
        {
            get;
            set;
        }

        [Import]
        public ILoggerFacade Logger
        {
            get;
            set;
        }

        [Import]
        public ILoadingIndicatorService LoadingIndicatorService
        {
            get;
            set;
        }

        [Import]
        public ISearchBarService SearchBarService
        {
            get;
            set;
        }

        [Import]
        public IRadio Radio
        {
            get;
            set;
        }

        public bool? LikeCurrentTrack
        {
            get { return _likeCurrentTrack; }
            set
            {
                if (_likeCurrentTrack != value)
                {
                    _likeCurrentTrack = value;

                    EnsureTrackStreamExists();

                    if (_loveHateTrackStream != null && _likeCurrentTrack.HasValue)
                    {
                        if (_likeCurrentTrack.Value)
                        {
                            _loveHateTrackStream.LikeCurrentTrack();
                        }

                        _dislikeCurrentTrack = false;
                    }

                    RaisePropertyChanged("LikeCurrentTrack", "DislikeCurrentTrack");
                }
            }
        }

        public bool? DislikeCurrentTrack
        {
            get
            {
                return _dislikeCurrentTrack;
            }
            set
            {
                if (_dislikeCurrentTrack != value)
                {
                    _dislikeCurrentTrack = value;

                    EnsureTrackStreamExists();

                    if (_loveHateTrackStream != null && _dislikeCurrentTrack.HasValue)
                    {
                        if (_dislikeCurrentTrack.Value)
                        {
                            _loveHateTrackStream.DislikeCurrentTrack();
                        }

                        _likeCurrentTrack = false;
                    }

                    RaisePropertyChanged("LikeCurrentTrack", "DislikeCurrentTrack");
                }
            }
        }

        public double? CurrentTrackRating
        {
            get
            {
                return _currentTrackRating;
            }
            set
            {
                if (_currentTrackRating != value)
                {
                    _currentTrackRating = value;
                    RaisePropertyChanged("CurrentTrackRating");

                    EnsureTrackStreamExists();

                    if (_loveHateTrackStream != null)
                    {
                        if (_currentTrackRating.HasValue)
                        {
                            _loveHateTrackStream.SetCurrentTrackRating(_currentTrackRating.Value * 5);
                        }
                    }
                }
            }
        }

        public bool HasTrack
        {
            get
            {
                return _hasTrack;
            }
            set
            {
                if (_hasTrack != value)
                {
                    _hasTrack = value;
                    RaisePropertyChanged("HasTrack");
                }
            }
        }

        #endregion Properties

        #region Methods

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            SearchBarService.Add<LoveHateView>(new SearchBarData
            {
                Category = "Love_Or_Hate",
                WatermarkText = "Love_Or_Hate_Watermark",
                AutoCompleteProvider = SuggestArtists
            });

            SearchBarService.SetActive(searchBar => searchBar.NavigationUri.OriginalString.StartsWith(typeof(LoveHateView).FullName));

            if (!string.IsNullOrEmpty(navigationContext.Parameters[SearchBar.IsFromSearchBarParameter]))
            {
                var artistName = navigationContext.Parameters[SearchBar.ValueParameter];
                Execute(artistName);
            }

            Radio.CurrentTrackChanged += OnCurrentTrackChanged;

            if (Radio.CurrentTrack != null)
            {
                HasTrack = true;
            }

            _loveHateTrackStream = Radio.CurrentTrackStream as LoveHateTrackStream;
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            Radio.CurrentTrackChanged -= OnCurrentTrackChanged;

            SearchBarService.Remove(
                searchBar => searchBar.NavigationUri.OriginalString.StartsWith(typeof(LoveHateView).FullName));
        }

        private void EnsureTrackStreamExists()
        {
            if (_loveHateTrackStream == null && Radio.CurrentTrack != null)
            {
                _loveHateTrackStream = new LoveHateTrackStream(Radio.CurrentTrack.Artist,
                                                               Radio,
                                                               Logger,
                                                               ToastService,
                                                               LoadingIndicatorService);
            }
        }

        private void ExecuteSkipCurrentTrack()
        {
            EnsureTrackStreamExists();

            if (_loveHateTrackStream != null && Radio.CurrentTrackStream != _loveHateTrackStream)
            {
                Radio.Play(_loveHateTrackStream);
            }
            else
            {
                if (Radio.CanGoToNextTrack)
                {
                    Radio.NextTrack();
                }
            }
        }

        private void OnCurrentTrackChanged(object sender, TrackChangedEventArgs e)
        {
            if (e.PreviousTrack != null)
            {
                _likeCurrentTrack = false;
                _dislikeCurrentTrack = false;
                CurrentTrackRating = null;

                RaisePropertyChanged("LikeCurrentTrack", "DislikeCurrentTrack");
            }

            if (e.CurrentTrack == null)
            {
                HasTrack = false;
            }
            else
            {
                if (_loveHateTrackStream != null && Radio.CurrentTrackStream != _loveHateTrackStream)
                {
                    Radio.Play(_loveHateTrackStream);
                }

                HasTrack = true;
            }
        }

        private void Execute(string artistName)
        {
            _loveHateTrackStream = new LoveHateTrackStream(artistName, Radio, Logger, ToastService, LoadingIndicatorService);

            Radio.Play(_loveHateTrackStream);
        }

        private IEnumerable<string> SuggestArtists(string query)
        {
            try
            {
                return SuggestArtistsService.GetSimilarArtists(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return new string[0];
        }

        #endregion Methods
    }
}