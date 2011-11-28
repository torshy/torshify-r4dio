using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using EightTracks;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks
{
    public class EightTracksRadioStationViewModel : NotificationObject
    {
        #region Fields

        private readonly IRadioStationContext _context;

        private ObservableCollection<string> _autocompleteSearchPhrases;
        private ObservableCollection<Mix> _recentMixes;
        private ObservableCollection<Mix> _searchMixes;
        private EightTracksSearchType _searchType;

        #endregion Fields

        #region Constructors

        public EightTracksRadioStationViewModel(IRadioStationContext context)
        {
            _context = context;
            _autocompleteSearchPhrases = new ObservableCollection<string>();
            _recentMixes = new ObservableCollection<Mix>();
            _searchMixes = new ObservableCollection<Mix>();

            SearchForMixCommand = new DelegateCommand<string>(ExecuteSearchForMix);
            PlayMixCommand = new DelegateCommand<Mix>(ExecutePlayMix);
            FillRecentMixes();
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<string> AutocompleteSearchPhrases
        {
            get
            {
                return _autocompleteSearchPhrases;
            }
        }

        public ICommand PlayMixCommand
        {
            get;
            private set;
        }

        public IEnumerable<Mix> RecentMixes
        {
            get
            {
                return _recentMixes;
            }
        }

        public ICommand SearchForMixCommand
        {
            get;
            private set;
        }

        public IEnumerable<Mix> SearchMixes
        {
            get
            {
                return _searchMixes;
            }
        }

        public EightTracksSearchType SearchType
        {
            get { return _searchType; }
            set
            {
                _searchType = value;
                RaisePropertyChanged("SearchType");
            }
        }

        #endregion Properties

        #region Methods

        public void UpdateAutoComplete(string text)
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory
                .StartNew(() =>
                {
                    using (var session = new EightTracksSession(EightTracksRadioStation.ApiKey))
                    {
                        TagsResponse response = session.Query<Tags>().Execute(1, text);
                        return response.Tags.Select(t => t.Name);
                    }
                })
                .ContinueWith(t =>
                {
                    _autocompleteSearchPhrases.Clear();
                    foreach (var tag in t.Result)
                    {
                        _autocompleteSearchPhrases.Add(tag);
                    }
                }, ui);
        }

        private void ExecutePlayMix(Mix mix)
        {
            EightTracksTrackManager.Instance.StartMix(mix);
        }

        private void ExecuteSearchForMix(string text)
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                using (var session = new EightTracksSession(EightTracksRadioStation.ApiKey))
                {
                    if (SearchType == EightTracksSearchType.Tag)
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
            })
            .ContinueWith(t =>
            {
                _searchMixes.Clear();
                foreach (var mix in t.Result)
                {
                    _searchMixes.Add(mix);
                }
            }, ui);
        }

        private void FillRecentMixes()
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
                {
                    using (var session = new EightTracksSession(EightTracksRadioStation.ApiKey))
                    {
                        var response = session.Query<Mixes>().GetMix(sorting: Mixes.Sort.Recent);
                        return response.Mixes;
                    }
                })
                .ContinueWith(t =>
                {
                    foreach (var mix in t.Result)
                    {
                        _recentMixes.Add(mix);
                        _searchMixes.Add(mix);
                    }
                }, ui);
        }

        #endregion Methods
    }
}