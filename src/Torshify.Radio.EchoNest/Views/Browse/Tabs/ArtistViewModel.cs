using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.EchoNest.Views.Browse.Tabs.Models;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;
using Torshify.Radio.Framework.Events;

namespace Torshify.Radio.EchoNest.Views.Browse.Tabs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public class ArtistViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private ArtistModel _currentArtist;

        #endregion Fields

        #region Constructors

        public ArtistViewModel()
        {
            PlayTracksCommand = new StaticCommand<IEnumerable>(ExecutePlayTracks);
            QueueTracksCommand = new StaticCommand<IEnumerable>(ExecuteQueueTracks);
            GoToAlbumCommand = new StaticCommand<TrackContainer>(ExecuteGoToAlbum);
            CommandBar = new CommandBar();
        }

        #endregion Constructors

        #region Properties

        public ICommandBar CommandBar
        {
            get;
            private set;
        }

        public ArtistModel CurrentArtist
        {
            get
            {
                return _currentArtist;
            }
            set
            {
                if (_currentArtist != value)
                {
                    _currentArtist = value;
                    RaisePropertyChanged("CurrentArtist");
                }
            }
        }

        [Import]
        public IRadio Radio
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
        public IRegionManager RegionManager
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
        public IEventAggregator EventAggregator
        {
            get;
            set;
        }

        public StaticCommand<IEnumerable> QueueTracksCommand
        {
            get;
            private set;
        }

        public StaticCommand<IEnumerable> PlayTracksCommand
        {
            get;
            private set;
        }

        public StaticCommand<TrackContainer> GoToAlbumCommand
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            CurrentArtist = new ArtistModel();
            CurrentArtist.Name = navigationContext.Parameters["artistName"];

            Task.Factory
              .StartNew(state =>
              {
                  var artist = (ArtistModel)state;
                  using (LoadingIndicatorService.EnterLoadingBlock())
                  {
                      artist.Albums = GetAlbums(artist);
                  }
              }, CurrentArtist);
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            CurrentArtist = null;
        }

        public void UpdateCommandBar(IEnumerable<Track> selectedTracks)
        {
            var tracks = selectedTracks.ToArray();

            CommandBar.Clear()
                .AddCommand(new CommandModel
                {
                    Content = "Play",
                    Icon = AppIcons.Play.ToImage(),
                    Command = PlayTracksCommand,
                    CommandParameter = tracks
                })
                .AddCommand(new CommandModel
                {
                    Content = "Queue",
                    Icon = AppIcons.Add.ToImage(),
                    Command = QueueTracksCommand,
                    CommandParameter = tracks
                })
                .AddSeparator();

            var lastItem = tracks.LastOrDefault();
            if (lastItem != null)
            {
                var payload = new ArtistRelatedCommandBarPayload(lastItem.Artist, CommandBar);
                EventAggregator.GetEvent<BuildArtistRelatedCommandBarEvent>().Publish(payload);
            }
        }

        private IEnumerable<TrackContainer> GetAlbums(ArtistModel artist)
        {
            var albumsByArtist = new List<TrackContainer>();
            var albumsContainingArtist = new List<TrackContainer>();
            var albums = Radio.GetAlbumsByArtist(artist.Name).ToArray();

            foreach (var album in albums)
            {
                if (album.Tracks.All(t => t.Artist.Equals(artist.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    albumsByArtist.Add(album);
                }
                else
                {
                    albumsContainingArtist.Add(album);
                }
            }

            if (!albums.Any())
            {
                ToastService.Show(new ToastData
                {
                    Message = "No search results found for " + artist.Name,
                    Icon = AppIcons.Information
                });
            }

            return albumsByArtist
                .OrderBy(a => a.Name)
                .ThenBy(a => a.Year)
                .Concat(albumsContainingArtist
                .OrderBy(a => a.Name)
                .ThenBy(a => a.Year)).ToArray();
        }

        private void ExecuteQueueTracks(IEnumerable tracks)
        {
            if (tracks == null)
                return;

            ITrackStream stream = tracks.OfType<Track>().ToTrackStream(CurrentArtist.Name);
            Radio.Queue(stream);

            ToastService.Show(new ToastData
            {
                Message = "Tracks queued",
                Icon = AppIcons.Add
            });
        }

        private void ExecutePlayTracks(IEnumerable tracks)
        {
            if (tracks == null)
                return;

            ITrackStream stream = tracks.OfType<Track>().ToTrackStream(CurrentArtist.Name);
            Radio.Play(stream);
        }

        private void ExecuteGoToAlbum(TrackContainer album)
        {
            UriQuery q = new UriQuery();
            q.Add("artistName", album.Owner.Name);
            q.Add("albumName", album.Name);
            RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof(AlbumView).FullName + q);
        }

        #endregion Methods
    }
}