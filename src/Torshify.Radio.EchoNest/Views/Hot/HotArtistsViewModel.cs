using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Windows.Threading;
using EchoNest;
using EchoNest.Artist;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.EchoNest.Views.Hot.Models;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Hot
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public class HotArtistsViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly ILoadingIndicatorService _loadingIndicator;
        private readonly ILoggerFacade _logger;
        private readonly Dispatcher _dispatcher;
        private readonly IRadio _radio;
        private readonly IToastService _toastService;

        private ObservableCollection<HotArtistModel> _artists;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public HotArtistsViewModel(
            IRadio radio,
            IToastService toastService,
            ILoadingIndicatorService loadingIndicator,
            ILoggerFacade logger,
            Dispatcher dispatcher)
        {
            _radio = radio;
            _toastService = toastService;
            _loadingIndicator = loadingIndicator;
            _logger = logger;
            _dispatcher = dispatcher;
            _artists = new ObservableCollection<HotArtistModel>();

            PlayAllTracksCommand = new DelegateCommand(ExecutePlayAllTracks);
            PlayTopTrackCommand = new DelegateCommand<HotArtistModel>(ExecutePlayTopTrack);
            QueueTopTrackCommand = new DelegateCommand<HotArtistModel>(ExecuteQueueTopTrack);
        }

        #endregion Constructors

        #region Properties

        public DelegateCommand PlayAllTracksCommand
        {
            get;
            private set;
        }

        public DelegateCommand<HotArtistModel> PlayTopTrackCommand
        {
            get;
            private set;
        }

        public DelegateCommand<HotArtistModel> QueueTopTrackCommand
        {
            get;
            private set;
        }

        public IEnumerable<HotArtistModel> Artists
        {
            get
            {
                return _artists;
            }
        }

        #endregion Properties

        #region Methods

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            Task.Factory
                .StartNew(() =>
                {
                    var response = MemoryCache.Default.Get("TopHotArtists") as TopHotttResponse;

                    if (response == null)
                    {
                        using (_loadingIndicator.EnterLoadingBlock())
                        {
                            using (var session = new EchoNestSession(EchoNestModule.ApiKey))
                            {
                                response = session.Query<TopHottt>().Execute(99, bucket:
                                                                                     ArtistBucket.Images |
                                                                                     ArtistBucket.Songs);

                                if (response != null)
                                {
                                    MemoryCache
                                        .Default
                                        .Add("TopHotArtists", response, DateTimeOffset.Now.AddHours(6));

                                    return response;
                                }
                            }
                        }
                    }

                    return response;
                })
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                        _toastService.Show("Unable to fetch artists");
                    }
                    else
                    {
                        if (task.Result != null)
                        {
                            if (task.Result.Status.Code == ResponseCode.Success)
                            {
                                var artists = task.Result.Artists.Select((a, i) =>
                                {
                                    string name = a.Name;
                                    string image = a.Images.Count > 0 ? a.Images[0].Url : null;
                                    string song = a.Songs.Count > 0 ? a.Songs[0].Title : null;
                                    return new HotArtistModel((i + 1), name, image, song);
                                });

                                const int numberOfObjectsPerPage = 10;
                                int numberOfObjectsTaken = 0;
                                int count = artists.Count();
                                int pageNumber = 0;
                                
                                while (numberOfObjectsTaken < count)
                                {
                                    IEnumerable<HotArtistModel> queryResultPage = artists
                                        .Skip(numberOfObjectsPerPage * pageNumber)
                                        .Take(numberOfObjectsPerPage).ToArray();

                                    numberOfObjectsTaken += queryResultPage.Count();
                                    pageNumber++;

                                    _dispatcher
                                        .BeginInvoke(
                                            new Action<IEnumerable<HotArtistModel>>(m => m.ForEach(model => _artists.Add(model))),
                                            DispatcherPriority.Background,
                                            queryResultPage);
                                }
                            }
                            else
                            {
                                _toastService.Show(task.Result.Status.Message);
                            }
                        }
                    }
                });
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void ExecutePlayAllTracks()
        {
            Task.Factory
                .StartNew(() => _radio.Play(new TopHotttTrackStream(_radio)))
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                    }
                });
        }

        private void ExecutePlayTopTrack(HotArtistModel artist)
        {
            GetTopTrackForArtistAsync(artist)
                .ContinueWith(task =>
                {
                    if (task.Result != null)
                    {
                        _radio.Play(task.Result.ToTrackStream(task.Result.Name, "Top hot artists"));
                    }
                    else
                    {
                        _toastService.Show("Unable to find track " + artist.HotSongTitle);
                    }
                });
        }

        private void ExecuteQueueTopTrack(HotArtistModel artist)
        {
            GetTopTrackForArtistAsync(artist)
                .ContinueWith(task =>
                {
                    if (task.Result != null)
                    {
                        _radio.Queue(task.Result.ToTrackStream(task.Result.Name, "Top hot artists"));
                        _toastService.Show(new ToastData
                        {
                            Message = "Queued " + task.Result.Name,
                            Icon = AppIcons.Add
                        });
                    }
                    else
                    {
                        _toastService.Show("Unable to find track " + artist.HotSongTitle);
                    }
                });
        }

        private Task<Track> GetTopTrackForArtistAsync(HotArtistModel artist)
        {
            return Task.Factory
                .StartNew(state =>
                {
                    HotArtistModel m = (HotArtistModel)state;
                    using (_loadingIndicator.EnterLoadingBlock())
                    {
                        var result = _radio.GetTracksByName(m.Name + " " + m.HotSongTitle);
                        var track = result.FirstOrDefault(
                            t => t.Artist.Equals(m.Name, StringComparison.InvariantCultureIgnoreCase));

                        if (track == null)
                        {
                            track = result.FirstOrDefault();
                        }

                        return track;
                    }
                }, artist)
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                        _toastService.Show("Unable to play track");
                    }

                    return task.Result;
                });
        }

        #endregion Methods
    }
}