using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using EightTracks;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    public abstract class MixListViewModel : NotificationObject
    {
        #region Fields

        protected ObservableCollection<Mix> _mixes;
        protected ObservableCollection<string> _tagFilterList;

        private Timer _deferredSearchTimer;
        private TaskScheduler _ui;

        #endregion Fields

        #region Constructors

        protected MixListViewModel()
        {
            _ui = TaskScheduler.FromCurrentSynchronizationContext();
            _mixes = new ObservableCollection<Mix>();
            _tagFilterList = new ObservableCollection<string>();
            _deferredSearchTimer = new Timer(750);
            _deferredSearchTimer.Elapsed += OnDeferredSearchTimerTick;
            ToggleTagFilterCommand = new StaticCommand<string>(ExecuteToggleTagFilter);
        }

        #endregion Constructors

        #region Properties

        public abstract HeaderInfo HeaderInfo
        {
            get;
        }

        [Import]
        public ILoadingIndicatorService LoadingIndicatorService
        {
            get;
            set;
        }

        public IEnumerable<Mix> Mixes
        {
            get { return _mixes; }
        }

        public IEnumerable<string> TagFilterList
        {
            get { return _tagFilterList; }
        }

        [Import]
        public IToastService ToastService
        {
            get;
            set;
        }

        public StaticCommand<string> ToggleTagFilterCommand
        {
            get;
            private set;
        }

        protected abstract Mixes.Sort SortType
        {
            get;
        }

        #endregion Properties

        #region Methods

        protected virtual void SearchForMixes(Mixes.Sort sortType)
        {
            Task.Factory.StartNew(() =>
            {
                using (LoadingIndicatorService.EnterLoadingBlock())
                {
                    using (var session = new EightTracksSession(EightTracksModule.ApiKey))
                    {
                        var response = session.Query<Mixes>().GetMix(sorting: sortType, filter: String.Join(",", TagFilterList), resultsPerPage: 25);
                        return response.Mixes;
                    }
                }
            })
            .ContinueWith(t =>
            {
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
            }, _ui);
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