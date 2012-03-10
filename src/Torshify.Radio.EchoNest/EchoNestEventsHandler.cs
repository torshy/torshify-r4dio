using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.EchoNest.Views.Browse.Tabs;
using Torshify.Radio.EchoNest.Views.LoveHate;
using Torshify.Radio.EchoNest.Views.Similar;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Events;
using System.Linq;

namespace Torshify.Radio.EchoNest
{
    public class EchoNestEventsHandler : IStartable
    {
        #region Properties

        [Import]
        private IEventAggregator _eventAggregator = null;

        [Import]
        private IRegionManager _regionManager = null;

        [Import]
        private IRadio _radio = null;

        [Import]
        private ILoadingIndicatorService _loadingIndicator = null;

        [Import]
        private IToastService _toastService = null;

        [Import]
        private ILoggerFacade _logger = null;

        #endregion Properties

        #region Methods

        public void Start()
        {
            _eventAggregator
                .GetEvent<BuildArtistRelatedCommandBarEvent>()
                .Subscribe(OnBuildArtistRelatedCommandBar, ThreadOption.PublisherThread);

            _eventAggregator
                .GetEvent<BuildAlbumRelatedCommandBarEvent>()
                .Subscribe(OnBuildAlbumRelatedCommandBar, ThreadOption.PublisherThread);

            AppCommands.GoToArtistCommand.RegisterCommand(new DelegateCommand<object>(ExecuteBrowseForArtist));
            AppCommands.GoToAlbumCommand.RegisterCommand(new DelegateCommand<object>(ExecuteBrowseForAlbum));
        }

        private void OnBuildArtistRelatedCommandBar(ArtistRelatedCommandBarPayload payload)
        {
            payload.CommandBar.AddCommand(
                new CommandModel
                {
                    Content = "Browse",
                    Command = new DelegateCommand<string>(ExecuteBrowseForArtist),
                    CommandParameter = payload.ArtistName,
                    Icon = AppIcons.Search.ToImage()
                });

            payload.CommandBar.AddCommand(
                new CommandModel
                {
                    Content = "Play",
                    Command = new DelegateCommand<string>(ExecutePlayArtist),
                    CommandParameter = payload.ArtistName,
                    Icon = AppIcons.Play.ToImage()
                });

            payload.CommandBar.AddCommand(
                new CommandModel
                {
                    Content = "Queue",
                    Command = new DelegateCommand<string>(ExecuteQueueArtist),
                    CommandParameter = payload.ArtistName,
                    Icon = AppIcons.Add.ToImage()
                });

            payload.CommandBar.AddCommand(
                new CommandModel
                {
                    Content = "Start radio",
                    Command = new DelegateCommand<string>(ExecuteStartArtistRadio),
                    CommandParameter = payload.ArtistName
                });

            payload.CommandBar.AddCommand(
                new CommandModel
                {
                    Content = "Find similar artists",
                    Command = new DelegateCommand<string>(ExecuteBrowseForSimilarArtists),
                    CommandParameter = payload.ArtistName
                });
        }

        private void OnBuildAlbumRelatedCommandBar(AlbumRelatedCommandBarPayload payload)
        {
            payload.CommandBar.AddCommand(
                new CommandModel
                {
                    Content = "Browse",
                    Command = new DelegateCommand<Tuple<string, string>>(ExecuteBrowseForAlbum),
                    CommandParameter = Tuple.Create(payload.ArtistName, payload.AlbumName),
                    Icon = AppIcons.Search.ToImage()
                });

            payload.CommandBar.AddCommand(
                new CommandModel
                {
                    Content = "Play",
                    Command = new DelegateCommand<Tuple<string, string>>(ExecutePlayArtistAlbum),
                    CommandParameter = Tuple.Create(payload.ArtistName, payload.AlbumName),
                    Icon = AppIcons.Play.ToImage()
                });
            
            payload.CommandBar.AddCommand(
                new CommandModel
                {
                    Content = "Queue",
                    Command = new DelegateCommand<Tuple<string, string>>(ExecuteQueueArtistAlbum),
                    CommandParameter = Tuple.Create(payload.ArtistName, payload.AlbumName),
                    Icon = AppIcons.Add.ToImage()
                });
        }

        private void ExecuteBrowseForArtist(object parameter)
        {
            var track = parameter as Track;

            if (track != null)
            {
                ExecuteBrowseForArtist(track.Artist);
            }

            var artistName = parameter as string;

            if (artistName != null)
            {
                ExecuteBrowseForArtist(artistName);
            }
        }

        private void ExecuteBrowseForArtist(string artistName)
        {
            if (!string.IsNullOrEmpty(artistName))
            {
                UriQuery query = new UriQuery();
                query.Add("artistName", artistName);
                _regionManager.RequestNavigate(AppRegions.ViewRegion, typeof(ArtistView).FullName + query);
            }
        }

        private void ExecutePlayArtist(string artistName)
        {
            if (string.IsNullOrEmpty(artistName))
            {
                return;
            }

            GetArtistTracksAsync(artistName)
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                    }
                    else
                    {
                        if (task.Result.Any())
                        {
                            _radio.Play(task.Result.ToTrackStream(artistName));
                        }
                        else
                        {
                            _toastService.Show("Unable to find track for artist " + artistName);
                        }
                    }
                });
        }

        private void ExecuteQueueArtist(string artistName)
        {
            GetArtistTracksAsync(artistName)
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                    }
                    else
                    {
                        if (task.Result.Any())
                        {
                            _radio.Queue(task.Result.ToTrackStream(artistName));
                        }
                        else
                        {
                            _toastService.Show("Unable to find track for artist " + artistName);
                        }
                    }
                });
        }

        private Task<IEnumerable<Track>> GetArtistTracksAsync(string artistName)
        {
            return Task<IEnumerable<Track>>.Factory
                .StartNew(state =>
                {
                    string name = state.ToString();
                    using (_loadingIndicator.EnterLoadingBlock())
                    {
                        var result = _radio.GetTracksByName(name)
                            .Where(t => t.Artist.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                            .ToArray();

                        return result;
                    }
                }, artistName);
        }

        private void ExecutePlayArtistAlbum(Tuple<string, string> artistAlbum)
        {
            if (artistAlbum == null)
            {
                return;
            }

            GetArtistAlbumTracksAsync(artistAlbum)
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                    }
                    else
                    {
                        var state = (Tuple<string, string>)task.AsyncState;

                        if (task.Result.Any())
                        {
                            _radio.Play(task.Result.ToTrackStream(state.Item2 + " by " + state.Item1));
                        }
                        else
                        {
                            _toastService.Show("Unable to find album " + state.Item2 + " by " + state.Item1);
                        }
                    }
                });
        }

        private void ExecuteQueueArtistAlbum(Tuple<string, string> artistAlbum)
        {
            if (artistAlbum == null)
            {
                return;
            }

            GetArtistAlbumTracksAsync(artistAlbum)
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                    }
                    else
                    {
                        var state = (Tuple<string, string>)task.AsyncState;

                        if (task.Result.Any())
                        {
                            _radio.Queue(task.Result.ToTrackStream(state.Item2 + " by " + state.Item1));
                        }
                        else
                        {
                            _toastService.Show("Unable to find album " + state.Item2 + " by " + state.Item1);
                        }
                    }
                });
        }

        private Task<IEnumerable<Track>> GetArtistAlbumTracksAsync(Tuple<string, string> artistAlbum)
        {
            return Task<IEnumerable<Track>>.Factory
                .StartNew(state =>
                {
                    var tuple = (Tuple<string, string>) state;
                    using (_loadingIndicator.EnterLoadingBlock())
                    {
                        var result = _radio.GetAlbumsByArtist(tuple.Item1)
                            .FirstOrDefault(t => t.Name.Equals(tuple.Item2, StringComparison.InvariantCultureIgnoreCase));

                        if (result != null)
                        {
                            return result.Tracks;
                        }
                        
                        return new Track[0];
                    }
                }, artistAlbum);
        }

        private void ExecuteStartArtistRadio(string artistName)
        {
            if (!string.IsNullOrEmpty(artistName))
            {
                UriQuery query = new UriQuery();
                query.Add(SearchBar.IsFromSearchBarParameter, "true");
                query.Add(SearchBar.ValueParameter, artistName);
                _regionManager.RequestNavigate(AppRegions.ViewRegion, typeof(LoveHateView).FullName + query);
            }
        }

        private void ExecuteBrowseForSimilarArtists(string artistName)
        {
            if (!string.IsNullOrEmpty(artistName))
            {
                UriQuery query = new UriQuery();
                query.Add(SearchBar.IsFromSearchBarParameter, "true");
                query.Add(SearchBar.ValueParameter, artistName);
                _regionManager.RequestNavigate(AppRegions.ViewRegion, typeof(MainStationView).FullName + query);
            }
        }

        private void ExecuteBrowseForAlbum(object parameter)
        {
            var track = parameter as Track;

            if (track != null)
            {
                ExecuteBrowseForAlbum(Tuple.Create(track.Artist, track.Album));
            }

            var container = parameter as TrackContainer;

            if (container != null)
            {
                ExecuteBrowseForAlbum(Tuple.Create(container.Owner.Name, container.Name));
            }

            var tuple = parameter as Tuple<string, string>;

            if (tuple != null)
            {
                ExecuteBrowseForAlbum(tuple);
            }
        }

        private void ExecuteBrowseForAlbum(Tuple<string, string> parameter)
        {
            if (!string.IsNullOrEmpty(parameter.Item1) && !string.IsNullOrEmpty(parameter.Item2))
            {
                UriQuery query = new UriQuery();
                query.Add("artistName", parameter.Item1);
                query.Add("albumName", parameter.Item2);
                _regionManager.RequestNavigate(AppRegions.ViewRegion, typeof(AlbumView).FullName + query);
            }
        }

        #endregion Methods
    }
}