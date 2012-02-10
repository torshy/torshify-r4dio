using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.EchoNest.Views.Browse.Tabs.Models;
using Torshify.Radio.Framework;

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

            return albumsByArtist
                .OrderBy(a => a.Name)
                .ThenBy(a => a.Year)
                .Concat(albumsContainingArtist
                .OrderBy(a => a.Name)
                .ThenBy(a => a.Year)).ToArray();
        }

        #endregion Methods
    }
}