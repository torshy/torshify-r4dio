using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.Framework;

namespace Torshify.Radio
{
    public class RadioStationContext : IRadioStationContext
    {
        #region Fields

        private readonly RadioNowPlayingViewModel _nowPlayingViewModel;
        private readonly IRadio _radio;
        private readonly IRegionManager _regionManager;

        private Func<IEnumerable<RadioTrack>> _getNextBatchProvider;
        private bool _getNextBatchProviderIsComplete;

        #endregion Fields

        #region Constructors

        public RadioStationContext(IRadio radio, IRegionManager regionManager, RadioNowPlayingViewModel nowPlayingViewModel)
        {
            _radio = radio;
            _regionManager = regionManager;
            _nowPlayingViewModel = nowPlayingViewModel;
        }

        #endregion Constructors

        #region Properties

        public IRadio Radio
        {
            get { return _radio; }
        }

        #endregion Properties

        #region Methods

        public void GetNextBatch()
        {
            if (_getNextBatchProvider != null && !_getNextBatchProviderIsComplete)
            {
                Task.Factory
                    .StartNew(_getNextBatchProvider)
                    .ContinueWith(t =>
                                      {
                                          if (!t.Result.Any())
                                          {
                                              _getNextBatchProviderIsComplete = true;
                                          }
                                          else
                                          {
                                              _getNextBatchProviderIsComplete = false;
                                              _nowPlayingViewModel.AddTracks(t.Result);
                                              _nowPlayingViewModel.PeekToNext(false);
                                          }
                                      });
            }
        }

        public void GoToStations()
        {
            ActivateView(RadioStandardViews.Stations);
        }

        public void GoToTracks()
        {
            ActivateView(RadioStandardViews.Tracks);
        }

        public void HideLoadingView()
        {
            if (_regionManager.Regions.ContainsRegionWithName(RegionNames.MainRegion))
            {
                ProgressBar progressBar = new ProgressBar();
                progressBar.IsIndeterminate = true;
                progressBar.SetResourceReference(FrameworkElement.StyleProperty, "ProgressBar_LoadingDots");

                IRegion region = _regionManager.Regions[RegionNames.MainRegionOverlay];
                var currentViews = region.Views.ToArray();

                foreach (var currentView in currentViews)
                {
                    region.Remove(currentView);
                }
            }
        }

        public void RemoveCurrentView()
        {
            if (_regionManager.Regions.ContainsRegionWithName(RegionNames.MainRegion))
            {
                IRegion region = _regionManager.Regions[RegionNames.MainRegion];

                ViewData currentStationView = region.GetView("CurrentStation") as ViewData;

                if (currentStationView != null)
                {
                    region.Remove(currentStationView);
                }
            }
        }

        public Task SetTrackProvider(Func<IEnumerable<RadioTrack>> getNextBatchProvider)
        {
            _getNextBatchProvider = getNextBatchProvider;
            ShowLoadingView();
            var ui = TaskScheduler.FromCurrentSynchronizationContext();

            return Task.Factory
                .StartNew(getNextBatchProvider)
                .ContinueWith(t =>
                                  {
                                      _nowPlayingViewModel.ClearTracks();

                                      if (!t.Result.Any())
                                      {
                                          _getNextBatchProviderIsComplete = true;
                                      }
                                      else
                                      {
                                          _getNextBatchProviderIsComplete = false;
                                          _nowPlayingViewModel.AddTracks(t.Result);
                                          _nowPlayingViewModel.MoveToNext();
                                      }
                                  })
                .ContinueWith(t => HideLoadingView(), ui);
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

        public void ShowLoadingView()
        {
            if (_regionManager.Regions.ContainsRegionWithName(RegionNames.MainRegion))
            {
                Border border = new Border();
                border.Background = Brushes.White;

                ProgressBar progressBar = new ProgressBar();
                progressBar.IsIndeterminate = true;
                progressBar.SetResourceReference(FrameworkElement.StyleProperty, "ProgressBar_LoadingDots");
                border.Child = progressBar;

                IRegion region = _regionManager.Regions[RegionNames.MainRegionOverlay];
                var currentViews = region.Views.ToArray();

                foreach (var currentView in currentViews)
                {
                    region.Remove(currentView);
                }

                region.Add(border);
                region.Activate(border);
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

        private ViewData GetCurrentView()
        {
            if (_regionManager.Regions.ContainsRegionWithName(RegionNames.MainRegion))
            {
                IRegion region = _regionManager.Regions[RegionNames.MainRegion];
                return region.GetView("CurrentStation") as ViewData;
            }

            return null;
        }

        #endregion Methods
    }
}