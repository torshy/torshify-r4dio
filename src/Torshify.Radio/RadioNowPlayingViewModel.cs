using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio
{
    public class RadioNowPlayingViewModel : NotificationObject, IRadioStationContext
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly IRadio _radio;
        private readonly IRegionManager _regionManager;

        private Func<IEnumerable<RadioTrack>> _getNextBatchProvider;
        private bool _getNextBatchProviderIsComplete;
        private ConcurrentQueue<RadioTrack> _playQueue;
        private TaskScheduler _uiTaskScheduler;

        #endregion Fields

        #region Constructors

        public RadioNowPlayingViewModel(IRadio radio, IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _radio = radio;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _radio.TrackComplete += OnTrackComplete;
            _playQueue = new ConcurrentQueue<RadioTrack>();
            _uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        #endregion Constructors

        #region Properties

        public RadioTrack CurrentTrack
        {
            get;
            private set;
        }

        public bool HasTracks
        {
            get { return CurrentTrack != null || _playQueue.Count > 0; }
        }

        public bool HasUpNext
        {
            get { return NextTrack != null; }
        }

        public RadioTrack NextTrack
        {
            get;
            private set;
        }

        public IRadio Radio
        {
            get { return _radio; }
        }

        #endregion Properties

        #region Methods

        public void AddTracks(IEnumerable<RadioTrack> tracks)
        {
            foreach (var radioTrack in tracks)
            {
                _playQueue.Enqueue(radioTrack);
            }
        }

        public void ClearTracks()
        {
            _playQueue = new ConcurrentQueue<RadioTrack>();

            CurrentTrack = null;
            NextTrack = null;

            RaisePropertyChanged("CurrentTrack", "NextTrack", "HasTracks", "HasUpNext");
        }

        public void GoToStations()
        {
            ActivateView(RadioStandardViews.Stations);
        }

        public void GoToTracks()
        {
            ActivateView(RadioStandardViews.Tracks);
        }

        public void MoveToNext()
        {
            bool success = false;

            while (!success)
            {
                RadioTrack track;
                if (_playQueue.TryDequeue(out track))
                {
                    try
                    {
                        if (track != null)
                        {
                            _radio.Load(track);
                            _radio.Play();
                            CurrentTrack = track;
                            success = true;
                            RaisePropertyChanged("CurrentTrack", "HasTracks");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    break;
                }
            }

            if (!success && _playQueue.IsEmpty)
            {
                var result = _getNextBatchProvider();

                if (result.Count() == 0)
                {
                    _getNextBatchProviderIsComplete = true;
                }
                else
                {
                    AddTracks(result);
                    MoveToNext();
                }
            }

            PeekToNext();
            RaisePropertyChanged("CurrentTrack", "HasTracks");
        }

        public void PeekToNext(bool getNextBatchIfNoMoreTracks = true)
        {
            RadioTrack track;

            if (_playQueue.TryPeek(out track))
            {
                NextTrack = track;
            }
            else
            {
                NextTrack = null;

                if (getNextBatchIfNoMoreTracks)
                {
                    GetNextBatch();
                }
            }

            RaisePropertyChanged("NextTrack", "HasUpNext");
        }

        public Task SetTrackProvider(Func<IEnumerable<RadioTrack>> getNextBatchProvider)
        {
            _getNextBatchProvider = getNextBatchProvider;
            _getNextBatchProviderIsComplete = false;

            var cts = new CancellationTokenSource();
            ShowLoadingView(cts);
            return Task.Factory
                .StartNew(getNextBatchProvider, cts.Token)
                .ContinueWith(t =>
                {
                    if (cts.Token.IsCancellationRequested)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                    }

                    ClearTracks();

                    if (!t.Result.Any())
                    {
                        _getNextBatchProviderIsComplete = true;
                    }
                    else
                    {
                        _getNextBatchProviderIsComplete = false;
                        AddTracks(t.Result);
                        MoveToNext();
                    }
                }, cts.Token)
                .ContinueWith(t =>
                {
                    HideLoadingView();

                    if (t.Status == TaskStatus.Faulted)
                    {
                        MoveToNext();
                    }

                }, cts.Token, TaskContinuationOptions.None, _uiTaskScheduler);
        }

        public void SetView(ViewData viewData)
        {
            if (viewData == null)
                return;

            if (_regionManager.Regions.ContainsRegionWithName(RegionNames.MainRegion))
            {
                IRegion region = _regionManager.Regions[RegionNames.MainRegion];

                ViewData currentStationView = region.GetView("CurrentStation") as ViewData;

                if (currentStationView != null)
                {
                    region.Remove(currentStationView);
                }

                region.Add(viewData, "CurrentStation");
                region.Activate(viewData);
            }
        }

        public void ShowLoadingView(CancellationTokenSource cts)
        {
            if (_regionManager.Regions.ContainsRegionWithName(RegionNames.MainRegion))
            {
                IRegion region = _regionManager.Regions[RegionNames.MainRegionOverlay];
                var currentViews = region.Views.ToArray();

                foreach (var currentView in currentViews)
                {
                    region.Remove(currentView);
                }

                LoadingScreen loadingScreen = new LoadingScreen();
                loadingScreen.CancelCommand = new DelegateCommand(() =>
                {
                    cts.Cancel();
                    HideLoadingView();
                });
                region.Add(loadingScreen);
                region.Activate(loadingScreen);
            }
        }

        private void ActivateView(string viewId)
        {
            if (_regionManager.Regions.ContainsRegionWithName(RegionNames.MainRegion))
            {
                IRegion region = _regionManager.Regions[RegionNames.MainRegion];

                object view = region.GetView(viewId);

                if (view != null)
                {
                    region.Activate(view);
                }
            }
        }

        private void GetNextBatch()
        {
            if (_getNextBatchProvider != null && !_getNextBatchProviderIsComplete)
            {
                var result = _getNextBatchProvider();

                if (result.Count() == 0)
                {
                    _getNextBatchProviderIsComplete = true;
                }
                else
                {
                    _getNextBatchProviderIsComplete = false;
                    AddTracks(result);
                    PeekToNext(false);
                }
            }
        }

        private void HideLoadingView()
        {
            if (_regionManager.Regions.ContainsRegionWithName(RegionNames.MainRegion))
            {
                IRegion region = _regionManager.Regions[RegionNames.MainRegionOverlay];
                var currentViews = region.Views.ToArray();

                foreach (var currentView in currentViews)
                {
                    region.Remove(currentView);
                }
            }
        }

        private void OnTrackComplete(object sender, TrackEventArgs e)
        {
            if (e.Track.Equals(CurrentTrack))
            {
                Task.Factory.StartNew(MoveToNext);
            }
        }

        #endregion Methods
    }
}