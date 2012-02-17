using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.EchoNest.Views.Browse.Tabs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SearchResultsViewModel : NotificationObject, IHeaderInfoProvider<HeaderInfo>, INavigationAware
    {
        #region Fields

        private ObservableCollection<Track> _results;

        #endregion Fields

        #region Constructors

        public SearchResultsViewModel()
        {
            _results = new ObservableCollection<Track>();

            HeaderInfo = new HeaderInfo
                         {
                             Title = "Results"
                         };

            CommandBar = new CommandBar();
            GoToAlbumCommand = new StaticCommand<Track>(ExecuteGoToAlbum);
            GoToArtistCommand = new StaticCommand<string>(ExecuteGoToArtist);
            PlayTracksCommand = new StaticCommand<IEnumerable>(ExecutePlayTracks);
            QueueTracksCommand = new StaticCommand<IEnumerable>(ExecuteQueueTracks);
        }

        #endregion Constructors

        #region Properties

        public HeaderInfo HeaderInfo
        {
            get;
            private set;
        }

        public ObservableCollection<Track> Results
        {
            get
            {
                return _results;
            }
        }

        [Import]
        public IRegionManager RegionManager
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

        [Import]
        public ILoggerFacade Logger
        {
            get;
            set;
        }

        public ICommandBar CommandBar
        {
            get; 
            private set;
        }

        public StaticCommand<IEnumerable> PlayTracksCommand
        {
            get;
            private set;
        }

        public StaticCommand<IEnumerable> QueueTracksCommand
        {
            get;
            private set;
        }

        public StaticCommand<string> GoToArtistCommand
        {
            get;
            private set;
        }

        public StaticCommand<Track> GoToAlbumCommand
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public void UpdateCommandBar(IEnumerable<Track> selectedItems)
        {
            CommandBar.Clear()
                .AddCommand(new CommandModel
                {
                    Content = "Play",
                    Icon = AppIcons.Play.ToImage(),
                    Command = PlayTracksCommand,
                    CommandParameter = selectedItems.ToArray()
                })
                .AddCommand(new CommandModel
                {
                    Content = "Queue",
                    Icon = AppIcons.Add.ToImage(),
                    Command = QueueTracksCommand,
                    CommandParameter = selectedItems.ToArray()
                })
                .AddSeparator();
        }

        void INavigationAware.OnNavigatedTo(NavigationContext context)
        {
            if (!string.IsNullOrEmpty(context.Parameters[SearchBar.IsFromSearchBarParameter]))
            {
                string value = context.Parameters[SearchBar.ValueParameter];
                ExecuteSearch(value);
            }
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext context)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext context)
        {
        }

        private void ExecuteSearch(string query)
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task
                .Factory
                .StartNew(state =>
                {
                    LoadingIndicatorService.Push();
                    return Radio.GetTracksByName(state.ToString());
                }, query)
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        ToastService.Show("Error while search for " + task.AsyncState);
                        Logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                    }
                    else
                    {
                        _results.Clear();

                        foreach (var track in task.Result)
                        {
                            _results.Add(track);
                        }
                    }

                    LoadingIndicatorService.Pop();
                }, ui);
        }

        private void ExecuteGoToArtist(string artistName)
        {
            UriQuery q = new UriQuery();
            q.Add("artistName", artistName);
            RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof(ArtistView).FullName + q);
        }

        private void ExecuteGoToAlbum(Track track)
        {
            UriQuery q = new UriQuery();
            q.Add("artistName", track.Artist);
            q.Add("albumName", track.Album);
            RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof(AlbumView).FullName + q);
        }

        private void ExecutePlayTracks(IEnumerable tracks)
        {
            if (tracks == null)
                return;

            ITrackStream stream = tracks.OfType<Track>().ToTrackStream("Browsing");
            Radio.Play(stream);
        }

        private void ExecuteQueueTracks(IEnumerable tracks)
        {
            if (tracks == null)
                return;

            ITrackStream stream = tracks.OfType<Track>().ToTrackStream("Browsing");
            Radio.Queue(stream);

            foreach (Track track in tracks)
            {
                Logger.Log("Queued " + track.Name, Category.Debug, Priority.Low);
            }

            ToastService.Show(new ToastData
            {
                Message = "Tracks queued",
                Icon = AppIcons.Add
            });
        }

        #endregion Methods
    }
}