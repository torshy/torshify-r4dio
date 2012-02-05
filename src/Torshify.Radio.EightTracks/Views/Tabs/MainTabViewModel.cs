using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using EightTracks;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(MainTabViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainTabViewModel : MixListViewModel, INavigationAware
    {
        #region Fields

        private HeaderInfo _headerInfo;

        #endregion Fields

        #region Constructors

        public MainTabViewModel()
        {
            _headerInfo = new HeaderInfo { Title = "Mixes" };
            _mixes = new ObservableCollection<Mix>();
        }

        #endregion Constructors

        #region Properties

        public override HeaderInfo HeaderInfo
        {
            get { return _headerInfo; }
        }

        protected override Mixes.Sort SortType
        {
            get { return global::EightTracks.Mixes.Sort.Random; }
        }

        #endregion Properties

        #region Methods

        bool INavigationAware.IsNavigationTarget(NavigationContext context)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext context)
        {
        }

        void INavigationAware.OnNavigatedTo(NavigationContext context)
        {
            if (!string.IsNullOrEmpty(context.Parameters[SearchBar.IsFromSearchBarParameter]))
            {
                string value = context.Parameters[SearchBar.ValueParameter];
                string type = context.Parameters["Type"];
                SearchForMixes(value, type);
            }
            else
            {
                SearchForRecentMixes();
            }
        }

        private void SearchForMixes(string text, string type)
        {
            _tagFilterList.Clear();

            if (type == "Tag")
            {
                _tagFilterList.Add(text);
            }

            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                using (LoadingIndicatorService.EnterLoadingBlock())
                {
                    using (var session = new EightTracksSession(EightTracksModule.ApiKey))
                    {
                        if (type == "Tag")
                        {
                            var response = session.Query<Mixes>().GetMix(tag: text);
                            _currentPage = response.Page;
                            _numberOfPages = response.TotalPages;
                            return response.Mixes;
                        }
                        else
                        {
                            var response = session.Query<Mixes>().GetMix(filter: text);
                            _currentPage = response.Page;
                            _numberOfPages = response.TotalPages;
                            return response.Mixes;
                        }
                    }
                }
            })
            .ContinueWith(t =>
            {
                GoToNextPageCommand.NotifyCanExecuteChanged();
                GoToPreviousPageCommand.NotifyCanExecuteChanged();

                _tags.Clear();

                foreach (var mix in t.Result)
                {
                    foreach (var tag in mix.TagListCacheAsArray)
                    {
                        var value = tag.Trim();
                        if (!_tags.Contains(value) && !_tagFilterList.Contains(value))
                        {
                            _tags.Add(value);
                        }
                    }
                }

                _mixes.Clear();
                foreach (var mix in t.Result)
                {
                    _mixes.Add(mix);
                }
            }, ui);
        }

        private void SearchForRecentMixes()
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                using (LoadingIndicatorService.EnterLoadingBlock())
                {
                    using (var session = new EightTracksSession(EightTracksModule.ApiKey))
                    {
                        var response = session.Query<Mixes>().GetMix(sorting: global::EightTracks.Mixes.Sort.Random, resultsPerPage: 25);
                        _currentPage = response.Page;
                        _numberOfPages = response.TotalPages;
                        return response.Mixes;
                    }
                }
            })
            .ContinueWith(t =>
            {
                GoToNextPageCommand.NotifyCanExecuteChanged();
                GoToPreviousPageCommand.NotifyCanExecuteChanged();

                if (t.Exception != null)
                {
                    Logger.Log("Error while fetching mixes: " + t.Exception, Category.Exception, Priority.Medium);
                    ToastService.Show("An error occurred while getting mixes");
                }
                else
                {
                    _tags.Clear();

                    foreach (var mix in t.Result)
                    {
                        foreach (var tag in mix.TagListCacheAsArray)
                        {
                            var value = tag.Trim();
                            if (!_tags.Contains(value) && !_tagFilterList.Contains(value))
                            {
                                _tags.Add(value);
                            }
                        }
                    }

                    if (!t.Result.Any())
                    {
                        ToastService.Show("No results found");
                        return;
                    }

                    _mixes.Clear();

                    foreach (var mix in t.Result)
                    {
                        _mixes.Add(mix);
                    }
                }
            }, ui);
        }

        #endregion Methods
    }
}