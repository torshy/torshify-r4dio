using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using EchoNest;
using EchoNest.Artist;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.EchoNest.Views.Style.Models;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Style
{
    [Export]
    [RegionMemberLifetime(KeepAlive = false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainStationViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly ILoadingIndicatorService _loadingIndicatorService;

        private ObservableCollection<TermModel> _moods;
        private TaskScheduler _scheduler;
        private ObservableCollection<TermModel> _styles;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public MainStationViewModel(ILoadingIndicatorService loadingIndicatorService)
        {
            _loadingIndicatorService = loadingIndicatorService;
            _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _styles = new ObservableCollection<TermModel>();
            _moods = new ObservableCollection<TermModel>();
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<TermModel> Moods
        {
            get { return _moods; }
        }

        public IEnumerable<TermModel> Styles
        {
            get { return _styles; }
        }

        #endregion Properties

        #region Methods

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            InitializeStyles();
            InitializeMoods();
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void InitializeMoods()
        {
            Task.Factory
                .StartNew(() => GetItems(ListTermsType.Mood))
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        return new TermModel[0];
                    }

                    // Load stuff from db, such as number of times its been used etc
                    return task.Result.Select(t => new TermModel(t.Name, ListTermsType.Mood));
                })
                .ContinueWith(task =>
                {
                    foreach (var termModel in task.Result)
                    {
                        _moods.Add(termModel);
                    }
                },_scheduler);
        }

        private void InitializeStyles()
        {
            Task.Factory
                .StartNew(() => GetItems(ListTermsType.Style))
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        return new TermModel[0];
                    }

                    // Load stuff from db, such as number of times its been used etc
                    return task.Result.Select(t => new TermModel(t.Name, ListTermsType.Style));
                })
                .ContinueWith(task =>
                {
                    foreach (var termModel in task.Result)
                    {
                        _styles.Add(termModel);
                    }
                },_scheduler);
        }

        private IEnumerable<ListTermsItem> GetItems(ListTermsType type)
        {
            using (_loadingIndicatorService.EnterLoadingBlock())
            {
                using (var session = new EchoNestSession(EchoNestModule.ApiKey))
                {
                    var response = session.Query<ListTerms>().Execute(type);

                    if (response.Status.Code == ResponseCode.Success)
                    {
                        return response.Terms;
                    }
                }

                return new ListTermsItem[0];
            }
        }

        #endregion Methods
    }
}