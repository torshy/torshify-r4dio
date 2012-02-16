using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Raven.Client;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.EchoNest.Views.Favorites.Tabs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = true)]
    public class FavoritesViewModel : NotificationObject, INavigationAware, IHeaderInfoProvider<HeaderInfo>
    {
        #region Fields

        private IList<Favorite> _favoriteList;

        #endregion Fields

        #region Constructors

        public FavoritesViewModel()
        {
            HeaderInfo = new HeaderInfo
            {
                Title = "Favorites"
            };

            PlayFavoriteCommand = new StaticCommand<Favorite>(ExecutePlayFavorite);
            PlayTrackCommand = new StaticCommand<Track>(ExecutePlayTrack);
        }

        #endregion Constructors

        #region Properties

        public HeaderInfo HeaderInfo
        {
            get;
            private set;
        }

        public StaticCommand<Favorite> PlayFavoriteCommand
        {
            get;
            private set;
        }

        public StaticCommand<Track> PlayTrackCommand
        {
            get;
            private set;
        }

        [ImportMany]
        public IEnumerable<IFavoriteHandler> FavoriteHandlers
        {
            get; 
            set;
        }

        [Import]
        public IDocumentStore DocumentStore
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

        public IList<Favorite> FavoriteList
        {
            get
            {
                return _favoriteList;
            }
            set
            {
                _favoriteList = value;
                RaisePropertyChanged("FavoriteList");
            }
        }

        #endregion Properties

        #region Methods

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            using (var session = DocumentStore.OpenSession())
            {
                FavoriteList = session.Query<Favorite>().ToList();
            }
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void ExecutePlayTrack(Track favorite)
        {
        }

        private void ExecutePlayFavorite(Favorite favorite)
        {
            var favoriteHandler = FavoriteHandlers.FirstOrDefault(f => f.CanHandleFavorite(favorite));

            if (favoriteHandler != null)
            {
                favoriteHandler.Play(favorite);
            }
            else
            {
                ToastService.Show("Unable to play favorite");
                Logger.Log("Unable to find favorite handler for type " + favorite.GetType() + " and id " + favorite.Id, Category.Warn, Priority.Medium);
            }
        }

        #endregion Methods
    }
}