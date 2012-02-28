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
using Torshify.Radio.Framework.Commands;

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
        private ObservableCollection<TermModel> _selectedMoods;
        private ObservableCollection<TermModel> _selectedStyles;
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
            _selectedMoods = new ObservableCollection<TermModel>();
            _selectedMoods.CollectionChanged += (sender, args) => RaisePropertyChanged("SelectedMoodsText");
            _selectedStyles = new ObservableCollection<TermModel>();
            _selectedStyles.CollectionChanged += (sender, args) => RaisePropertyChanged("SelectedStylesText");
            ToggleStyleCommand = new StaticCommand<TermModel>(ExecuteToggleStyle);
            ToggleMoodCommand = new StaticCommand<TermModel>(ExecuteToggleMood);
        }

        #endregion Constructors

        #region Properties

        public StaticCommand<TermModel> ToggleStyleCommand
        {
            get;
            private set;
        }

        public StaticCommand<TermModel> ToggleMoodCommand
        {
            get;
            private set;
        }

        public IEnumerable<TermModel> Moods
        {
            get
            {
                return _moods;
            }
        }

        public IEnumerable<TermModel> Styles
        {
            get
            {
                return _styles;
            }
        }

        public IEnumerable<TermModel> SelectedMoods
        {
            get
            {
                return _selectedMoods;
            }
        }

        public IEnumerable<TermModel> SelectedStyles
        {
            get
            {
                return _selectedStyles;
            }
        }

        public string SelectedStylesText
        {
            get
            {
                if (_selectedStyles.Count > 0)
                {
                    return "(" + string.Join(", ", _selectedStyles.Select(s => s.Name)) + ")";
                }

                return string.Empty;
            }
        }

        public string SelectedMoodsText
        {
            get
            {
                if (_selectedMoods.Count > 0)
                {
                    return "(" + string.Join(", ", _selectedMoods.Select(s => s.Name)) + ")";
                }

                return string.Empty;
            }
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

        private void ExecuteToggleMood(TermModel term)
        {
            if (_selectedMoods.Contains(term))
            {
                _selectedMoods.Remove(term);
            }
            else
            {
                _selectedMoods.Add(term);
            }
        }

        private void ExecuteToggleStyle(TermModel term)
        {
            if (_selectedStyles.Contains(term))
            {
                _selectedStyles.Remove(term);
            }
            else
            {
                _selectedStyles.Add(term);
            }
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
                }, _scheduler);
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
                }, _scheduler);
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