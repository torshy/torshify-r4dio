using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using EchoNest;
using EchoNest.Artist;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Browse
{
    [Export(typeof(ArtistBrowseViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ArtistBrowseViewModel : NotificationObject, INavigationAware, IRegionMemberLifetime
    {
        #region Fields

        private readonly IRadio _radio;

        private ArtistModel _currentArtist;
        private bool _isLoading;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public ArtistBrowseViewModel(IRadio radio)
        {
            _radio = radio;

            PlayCommand = new DelegateCommand<object>(ExecutePlay);
            QueueCommand = new DelegateCommand<object>(ExecuteQueue);
        }

        #endregion Constructors

        #region Properties

        public ArtistModel CurrentArtist
        {
            get { return _currentArtist; }
            set
            {
                if (_currentArtist != value)
                {
                    _currentArtist = value;
                    RaisePropertyChanged("CurrentArtist");
                }
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    RaisePropertyChanged("IsLoading");
                }
            }
        }

        public bool KeepAlive
        {
            get { return false; }
        }

        public ICommand PlayCommand
        {
            get;
            private set;
        }

        public ICommand QueueCommand
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            string artistName = navigationContext.Parameters["name"];

            ArtistModel artistModel = new ArtistModel();
            artistModel.Name = artistName;
            CurrentArtist = artistModel;

            Task.Factory
                .StartNew(state =>
                {
                    var artist = (ArtistModel)state;
                    IsLoading = true;
                    artist.Albums = GetAlbums(artist);
                    IsLoading = false;
                }, artistModel);

            Task.Factory
                .StartNew(state =>
                {
                    var artist = (ArtistModel)state;
                    using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
                    {
                        var profile = session.Query<Profile>().Execute(
                            artist.Name,
                            ArtistBucket.News |
                            ArtistBucket.Images |
                            ArtistBucket.Biographies |
                            ArtistBucket.Blogs |
                            ArtistBucket.Video);

                        if (profile.Status.Code == ResponseCode.Success)
                        {
                            artist.News = profile.Artist.News;
                            artist.Images = profile.Artist.Images;
                            artist.Image = profile.Artist.Images.FirstOrDefault();
                            artist.Biographies = profile.Artist.Biographies;
                            artist.Biography = profile.Artist.Biographies.FirstOrDefault();
                            artist.Blogs = profile.Artist.Blogs;
                            artist.Videos = profile.Artist.Videos;
                        }
                    }
                }, artistModel);
        }

        private void ExecutePlay(object parameter)
        {
            var artist = parameter as ArtistModel;
            if (artist != null)
            {
                _radio.CurrentContext.SetTrackProvider(() =>
                {
                    List<RadioTrack> tracks = new List<RadioTrack>();

                    foreach (var album in artist.Albums)
                    {
                        tracks.AddRange(album.Tracks);
                    }

                    return tracks;
                });
            }

            var container = parameter as RadioTrackContainer;
            if (container != null)
            {
                _radio.CurrentContext.SetTrackProvider(() => container.Tracks);
            }

            var track = parameter as RadioTrack;
            if (track != null)
            {
                List<RadioTrack> tracks = new List<RadioTrack>();

                bool foundTrack = false;
                foreach (var album in CurrentArtist.Albums)
                {
                    if (album.Tracks.Contains(track))
                    {
                        var remaining = album.Tracks.SkipWhile(t => t != track).ToArray();
                        tracks.AddRange(remaining);
                        foundTrack = true;
                    }
                    else if (foundTrack)
                    {
                        tracks.AddRange(album.Tracks);
                    }
                }

                if (tracks.Count > 0)
                {
                    _radio.CurrentContext.SetTrackProvider(() => tracks);
                }
            }
        }

        private void ExecuteQueue(object parameter)
        {
        }

        private IEnumerable<RadioTrackContainer> GetAlbums(ArtistModel artist)
        {
            var albumsByArtist = new List<RadioTrackContainer>();
            var albumsContainingArtist = new List<RadioTrackContainer>();
            var albums = _radio.GetAlbumsByArtist(artist.Name).ToArray();

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

            return albumsByArtist.OrderBy(a => a.Name).Concat(albumsContainingArtist.OrderBy(a => a.Name)).ToArray();
        }

        #endregion Methods
    }
}