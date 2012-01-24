using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using EightTracks;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(MainTabViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainTabViewModel : NotificationObject, IHeaderInfoProvider<HeaderInfo>, INavigationAware
    {
        #region Fields

        private ObservableCollection<Mix> _mixes;

        #endregion Fields

        #region Constructors

        public MainTabViewModel()
        {
            HeaderInfo = new HeaderInfo { Title = "Mixes" };
            _mixes = new ObservableCollection<Mix>();
        }

        #endregion Constructors

        #region Properties

        public HeaderInfo HeaderInfo
        {
            get;
            private set;
        }

        public IEnumerable<Mix> Mixes
        {
            get { return _mixes; }
        }

        [Import]
        public ILoadingIndicatorService LoadingIndicatorService
        {
            get; 
            set;
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
                            return response.Mixes;
                        }
                        else
                        {
                            var response = session.Query<Mixes>().GetMix(filter: text);
                            return response.Mixes;
                        }
                    }
                }
            })
            .ContinueWith(t =>
            {
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
                        var response = session.Query<Mixes>().GetMix();
                        return response.Mixes;
                    }
                }
            })
            .ContinueWith(t =>
            {
                _mixes.Clear();

                foreach (var mix in t.Result)
                {
                    _mixes.Add(mix);
                }
            }, ui);
        }

        #endregion Methods
    }
}