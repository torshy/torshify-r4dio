using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Radio.Core.Views.Settings;
using Torshify.Radio.Core.Views.Stations;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;
using System.Linq;

namespace Torshify.Radio.Core.Views
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private ObservableCollection<string> _autoCompleteList;

        #endregion Fields

        #region Constructors

        public MainViewModel()
        {
            _autoCompleteList = new ObservableCollection<string>();

            SearchBarLoadingIndicatorService = new LoadingIndicatorService();
            NavigateBackCommand = new AutomaticCommand(ExecuteNavigateBack, CanExecuteNavigateBack);
            NavigateForwardCommand = new AutomaticCommand(ExecuteNavigateForward, CanExecuteNavigateForward);
            NavigateToHomeCommand = new AutomaticCommand(ExecuteNavigateToHome, CanExecuteNavigateToHome);
            NavigateToSettingsCommand = new AutomaticCommand(ExecuteNavigateToSettings, CanExecuteNavigateToSettings);
            SearchCommand = new AutomaticCommand<string>(ExecuteSearch, CanExecuteSearch);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand NavigateToSettingsCommand
        {
            get; private set;
        }

        public IEnumerable<string> AutoCompleteList
        {
            get
            {
                return _autoCompleteList;
            }
        }

        public AutomaticCommand NavigateBackCommand
        {
            get;
            private set;
        }

        public AutomaticCommand NavigateForwardCommand
        {
            get;
            private set;
        }

        public AutomaticCommand NavigateToHomeCommand
        {
            get;
            private set;
        }

        public ILoadingIndicatorService SearchBarLoadingIndicatorService
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
        public IRegionManager RegionManager
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
        public ILoggerFacade Logger
        {
            get;
            set;
        }

        public AutomaticCommand<string> SearchCommand
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public void UpdateAutoCompleteList(string text)
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task<IEnumerable<string>>
                .Factory
                .StartNew(() =>
                {
                    try
                    {
                        using (SearchBarLoadingIndicatorService.EnterLoadingBlock())
                        {
                            return SearchBarService.Current.Data.AutoCompleteProvider(text);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e.Message, Category.Exception, Priority.Medium);
                    }

                    return new string[0];
                })
                .ContinueWith(t =>
                {
                    _autoCompleteList.Clear();

                    foreach (var phrase in t.Result)
                    {
                        _autoCompleteList.Add(phrase);
                    }
                }, ui);
        }

        private bool CanExecuteNavigateToSettings()
        {
            var entry = RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.CurrentEntry;

            if (entry != null &&
                entry.Uri.OriginalString ==
                typeof(SettingsView).FullName)
            {
                return false;
            }

            return true;
        }

        private void ExecuteNavigateToSettings()
        {
            RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof(SettingsView).FullName);
        }

        private bool CanExecuteNavigateToHome()
        {
            var entry = RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.CurrentEntry;
            
            return
                entry != null &&
                entry.Uri.OriginalString !=
                typeof(StationsView).FullName;
        }

        private void ExecuteNavigateToHome()
        {
            RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof(StationsView).FullName);
        }

        private bool CanExecuteNavigateBack()
        {
            return RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.CanGoBack;
        }

        private bool CanExecuteNavigateForward()
        {
            return RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.CanGoForward;
        }

        private bool CanExecuteSearch(string phrase)
        {
            return !string.IsNullOrEmpty(phrase);
        }

        private void ExecuteNavigateBack()
        {
            RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.GoBack();
        }

        private void ExecuteNavigateForward()
        {
            RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.GoForward();
        }

        private void ExecuteSearch(string phrase)
        {
            var searchBar = SearchBarService.Current;

            if (searchBar != null)
            {
                UriQuery query = new UriQuery(searchBar.Parameters.ToString());
                query.Add(SearchBar.ValueParameter, phrase);
                query.Add(SearchBar.IsFromSearchBarParameter, "true");

                IRegion viewRegion = RegionManager.Regions[AppRegions.ViewRegion];
                IRegionNavigationService navigation = viewRegion.NavigationService;

                if (navigation.Journal.CurrentEntry != null)
                {
                    if (navigation.Journal.CurrentEntry.Uri.OriginalString.StartsWith(searchBar.NavigationUri.OriginalString))
                    {
                        INavigationAware navAware = null;
                        var activeView = viewRegion.ActiveViews.FirstOrDefault();

                        if (activeView is INavigationAware)
                        {
                            navAware = activeView as INavigationAware;
                        }
                        else
                        {
                            FrameworkElement element = activeView as FrameworkElement;

                            if (element != null && element.DataContext is INavigationAware)
                            {
                                navAware = element.DataContext as INavigationAware;
                            }
                        }

                        if (navAware != null)
                        {
                            navAware.OnNavigatedFrom(new NavigationContext(navigation,
                                                                           navigation.Journal.CurrentEntry.Uri));
                            navAware.OnNavigatedTo(new NavigationContext(navigation,
                                                                         new Uri(
                                                                             searchBar.NavigationUri.ToString() + query,
                                                                             UriKind.RelativeOrAbsolute)));
                            return;
                        }
                    }
                }
                
                RegionManager.RequestNavigate(AppRegions.ViewRegion, searchBar.NavigationUri.ToString() + query);
            }
        }

        #endregion Methods
    }
}