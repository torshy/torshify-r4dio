using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using EightTracks;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    public abstract class MixListViewModel : NotificationObject, IHeaderInfoProvider<HeaderInfo>
    {
        #region Fields

        private const int OneMegaByte = 1048576;

        protected ObservableCollection<Mix> _mixes;
        protected ObservableCollection<string> _tagFilterList;
        protected ObservableCollection<string> _tags;
        protected int? _currentPage;
        protected int? _numberOfPages;

        private Timer _deferredSearchTimer;
        private TaskScheduler _ui;

        #endregion Fields

        #region Constructors

        protected MixListViewModel()
        {
            _ui = TaskScheduler.FromCurrentSynchronizationContext();
            _mixes = new ObservableCollection<Mix>();
            _tagFilterList = new ObservableCollection<string>();
            _tags = new ObservableCollection<string>();
            _deferredSearchTimer = new Timer(750);
            _deferredSearchTimer.Elapsed += OnDeferredSearchTimerTick;
            ToggleTagFilterCommand = new StaticCommand<string>(ExecuteToggleTagFilter);
            PlayMixCommand = new StaticCommand<Mix>(ExecutePlayMix);
            QueueMixCommand = new StaticCommand<Mix>(ExecuteQueueMix);
            GoToNextPageCommand = new ManualCommand(ExecuteGoToNextPage, CanExecuteGoToNextPage);
            GoToPreviousPageCommand = new ManualCommand(ExecuteGoToPreviousPage, CanExecuteGoToPreviousPage);
        }

        #endregion Constructors

        #region Properties

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
        public IRadio Radio
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

        public ManualCommand GoToNextPageCommand
        {
            get;
            private set;
        }

        public ManualCommand GoToPreviousPageCommand
        {
            get;
            private set;
        }

        public StaticCommand<Mix> PlayMixCommand
        {
            get;
            private set;
        }

        public StaticCommand<Mix> QueueMixCommand
        {
            get;
            private set;
        }

        public StaticCommand<string> ToggleTagFilterCommand
        {
            get;
            private set;
        }

        public abstract HeaderInfo HeaderInfo
        {
            get;
        }

        public IEnumerable<Mix> Mixes
        {
            get { return _mixes; }
        }

        public IEnumerable<string> Tags
        {
            get { return _tags; }
        }

        public IEnumerable<string> TagFilterList
        {
            get { return _tagFilterList; }
        }

        protected abstract Mixes.Sort SortType
        {
            get;
        }

        #endregion Properties

        #region Methods

        protected virtual void SearchForMixes(Mixes.Sort sortType, int page = 1)
        {
            Task.Factory.StartNew(() =>
            {
                using (LoadingIndicatorService.EnterLoadingBlock())
                {
                    using (var session = new EightTracksSession(EightTracksModule.ApiKey))
                    {
                        session.SetHttpClientMaxResponseContentBufferSize(OneMegaByte);
                        var response = session.Query<Mixes>().GetMix(
                            sorting: sortType, 
                            filter: String.Join("+", TagFilterList),
                            page: page,
                            resultsPerPage: 25);

                        if (response != null)
                        {
                            _currentPage = response.Page;
                            _numberOfPages = response.TotalPages;
                            return response.Mixes;
                        }
                    }
                }

                return null;
            })
            .ContinueWith(t =>
            {
                GoToNextPageCommand.NotifyCanExecuteChanged();
                GoToPreviousPageCommand.NotifyCanExecuteChanged();

                if  (t.Exception != null)
                {
                    ToastService.Show(t.Exception.Message);
                    Logger.Log(t.Exception.ToString(), Category.Exception, Priority.Medium);
                    return;
                }
                
                if (t.Result != null)
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

                    if (!t.Result.Any() && SortType != global::EightTracks.Mixes.Sort.Random)
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
                else
                {
                    // TODO : Notify user
                }
            }, _ui);
        }

        private bool CanExecuteGoToNextPage()
        {
            return _currentPage.HasValue && _currentPage < _numberOfPages;
        }

        private bool CanExecuteGoToPreviousPage()
        {
            return _currentPage.HasValue && _currentPage > 1;
        }

        private void ExecuteGoToNextPage()
        {
            SearchForMixes(SortType, _currentPage.GetValueOrDefault() + 1);
        }

        private void ExecuteGoToPreviousPage()
        {
            SearchForMixes(SortType, _currentPage.GetValueOrDefault() - 1);
        }

        private void ExecutePlayMix(Mix mix)
        {
            Radio.Play(new EightTracksMixTrackStream(mix, ToastService));
        }

        private void ExecuteQueueMix(Mix mix)
        {
            Radio.Queue(new EightTracksMixTrackStream(mix, ToastService));
            ToastService.Show(new ToastData
            {
                Message = "Queued " + mix.Name,
                Icon = AppIcons.Add
            });
        }

        private void ExecuteToggleTagFilter(string tagFilter)
        {
            tagFilter = tagFilter.Trim();

            if (_tagFilterList.Contains(tagFilter))
            {
                _tagFilterList.Remove(tagFilter);
            }
            else
            {
                _tagFilterList.Add(tagFilter);
            }

            if (_tags.Contains(tagFilter))
            {
                _tags.Remove(tagFilter);
            }

            _deferredSearchTimer.Stop();
            _deferredSearchTimer.Start();
        }

        private void OnDeferredSearchTimerTick(object sender, ElapsedEventArgs e)
        {
            _deferredSearchTimer.Stop();
            SearchForMixes(SortType);
        }

        #endregion Methods
    }
}