using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using EightTracks;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(TagsTabViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TagsTabViewModel : NotificationObject, IHeaderInfoProvider<HeaderInfo>
    {
        #region Fields

        private int? _currentMixPage;
        private ObservableCollection<Mix> _mixes;
        private int? _nextTagPage;
        private int? _numberOfMixPages;
        private int? _previousTagPage;
        private ObservableCollection<Tag> _selectedTags;
        private ObservableCollection<Tag> _tags;
        private TaskScheduler _ui;

        #endregion Fields

        #region Constructors

        public TagsTabViewModel()
        {
            HeaderInfo = new HeaderInfo
                         {
                             Title = "Tags",
                             IsSelectedAction = isSelected =>
                                                {
                                                    if (!Tags.Any() && isSelected)
                                                    {
                                                        GetTags();
                                                    }
                                                }
                         };

            _mixes = new ObservableCollection<Mix>();
            _tags = new ObservableCollection<Tag>();
            _selectedTags = new ObservableCollection<Tag>();
            _ui = TaskScheduler.FromCurrentSynchronizationContext();
            ToggleTagCommand = new StaticCommand<Tag>(ExecuteToggleTag);
            GoToNextPageCommand = new ManualCommand(ExecuteGoToNextPage, CanExecuteGoToNextPage);
            GoToPreviousPageCommand = new ManualCommand(ExecuteGoToPreviousPage, CanExecuteGoToPreviousPage);
            GoToNextTagPageCommand = new ManualCommand(ExecutGoToNextTagPage, CanExecuteGoToNextTagPage);
            GoToPreviousTagPageCommand = new ManualCommand(ExecuteGoToPreviousTagPage, CanExecuteGoToPreviousTagPage);
        }

        #endregion Constructors

        #region Properties

        [Import]
        public ILoadingIndicatorService LoadingIndicatorService
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

        public ManualCommand GoToPreviousTagPageCommand
        {
            get;
            private set;
        }

        public ManualCommand GoToNextTagPageCommand
        {
            get;
            private set;
        }

        public HeaderInfo HeaderInfo
        {
            get;
            private set;
        }

        public StaticCommand<Tag> ToggleTagCommand
        {
            get;
            private set;
        }

        public IEnumerable<Mix> Mixes
        {
            get { return _mixes; }
        }

        public IEnumerable<Tag> Tags
        {
            get { return _tags; }
        }

        public IEnumerable<Tag> TagFilterList
        {
            get { return _selectedTags; }
        }

        public IEnumerable<string>  TagStringFilterList
        {
            get { return _selectedTags.Select(x => x.Name); }
        }

        #endregion Properties

        #region Methods

        private void GetMixes(int page = 1, string filter = null)
        {
            Task.Factory.StartNew(() =>
                {
                    using (LoadingIndicatorService.EnterLoadingBlock())
                    {
                        using (var session = new EightTracksSession(EightTracksModule.ApiKey))
                        {
                            var response = session.Query<Mixes>().GetMix(
                                sorting:global::EightTracks.Mixes.Sort.Popular,
                                filter: filter,
                                page: page,
                                resultsPerPage: 25);

                            if (response != null)
                            {
                                _currentMixPage = response.Page;
                                _numberOfMixPages = response.TotalPages;

                                return response.Mixes;
                            }
                        }
                    }

                    return null;
                })
                .ContinueWith(t =>
                {
                    RefreshCommands();

                    if (t.Result != null)
                    {
                        _mixes.Clear();

                        foreach (var mix in t.Result)
                        {
                            _mixes.Add(mix);
                        }
                    }
                }, _ui);
        }

        private bool CanExecuteGoToPreviousTagPage()
        {
            return _previousTagPage.HasValue;
        }

        private void ExecuteGoToPreviousTagPage()
        {
            GetTags(page: _previousTagPage.GetValueOrDefault(1));
        }

        private bool CanExecuteGoToNextTagPage()
        {
            return _nextTagPage.HasValue;
        }

        private void ExecutGoToNextTagPage()
        {
            GetTags(page: _nextTagPage.GetValueOrDefault(1));
        }

        private void ExecuteToggleTag(Tag tag)
        {
            AddOrRemoveFromSelectedList(tag);
            GetMixes(
                page:_currentMixPage.GetValueOrDefault(1),
                filter: string.Join("+", TagFilterList.Select(x => x.Name)));
        }

        private void AddOrRemoveFromSelectedList(Tag tag)
        {
            if (_selectedTags.Contains(tag))
            {
                _selectedTags.Remove(tag);
            }
            else
            {
                _selectedTags.Add(tag);
            }
        }

        private void GetTags(int page = 1)
        {
            Task.Factory.StartNew(() =>
            {
                using (LoadingIndicatorService.EnterLoadingBlock())
                {
                    using (var session = new EightTracksSession(EightTracksModule.ApiKey))
                    {
                        var response = session.Query<Tags>().Execute(
                            page:page);

                        if (response != null)
                        {
                            if (!response.NextPage.IsNull)
                            {
                                _nextTagPage = response.NextPage.Number;
                            }
                            else
                            {
                                _nextTagPage = null;
                            }

                            if (!response.PreviousPage.IsNull)
                            {
                                _previousTagPage = response.PreviousPage.Number;
                            }
                            else
                            {
                                _previousTagPage = null;
                            }

                            return response.Tags;
                        }
                    }
                }

                return null;
            })
            .ContinueWith(t =>
            {
                _tags.Clear();

                foreach (var tag in t.Result)
                {
                    _tags.Add(tag);
                }

                RefreshCommands();
            }, _ui);
        }

        private bool CanExecuteGoToNextPage()
        {
            return _currentMixPage.HasValue && _currentMixPage < _numberOfMixPages;
        }

        private bool CanExecuteGoToPreviousPage()
        {
            return _currentMixPage.HasValue && _currentMixPage > 1;
        }

        private void ExecuteGoToNextPage()
        {
            GetMixes(_currentMixPage.GetValueOrDefault() + 1, string.Join("+", TagFilterList.Select(x => x.Name)));
        }

        private void ExecuteGoToPreviousPage()
        {
            GetMixes(_currentMixPage.GetValueOrDefault() - 1, string.Join("+", TagFilterList.Select(x => x.Name)));
        }

        private void RefreshCommands()
        {
            GoToNextPageCommand.NotifyCanExecuteChanged();
            GoToPreviousPageCommand.NotifyCanExecuteChanged();
            GoToNextTagPageCommand.NotifyCanExecuteChanged();
            GoToPreviousTagPageCommand.NotifyCanExecuteChanged();
        }

        #endregion Methods
    }
}