using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Browse
{
    [Export(typeof(SearchResultsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SearchResultsViewModel : NotificationObject, INavigationAware, IRegionMemberLifetime
    {
        #region Fields

        private readonly IRadio _radio;

        private bool _isLoading;
        private IRegionNavigationService _navService;
        private ObservableCollection<RadioTrack> _results;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public SearchResultsViewModel(IRadio radio)
        {
            _radio = radio;
            _results = new ObservableCollection<RadioTrack>();

            GoToArtistCommand = new DelegateCommand<string>(ExecuteGoToArtist);
        }

        #endregion Constructors

        #region Properties

        public ICommand GoToArtistCommand
        {
            get;
            private set;
        }

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            private set
            {
                _isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        public bool KeepAlive
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<RadioTrack> Results
        {
            get
            {
                return _results;
            }
        }

        #endregion Properties

        #region Methods

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navService = navigationContext.NavigationService;

            var query = navigationContext.Parameters["query"];

            if (!string.IsNullOrEmpty(query))
            {
                ExecuteSearch(query);
            }
        }

        private void ExecuteGoToArtist(string artistName)
        {
            UriQuery uri = new UriQuery();
            uri.Add("name", artistName);

            _navService.RequestNavigate(new Uri(typeof(ArtistBrowseView).FullName + uri, UriKind.Relative));
        }

        private void ExecuteSearch(string query)
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory
                .StartNew(() =>
                {
                    IsLoading = true;
                    return _radio.GetTracksByName(query, 0, 32);
                })
                .ContinueWith(t =>
                {
                    _results.Clear();
                    foreach (var radioTrack in t.Result)
                    {
                        _results.Add(radioTrack);
                    }

                    IsLoading = false;
                }, ui);
        }

        #endregion Methods
    }
}