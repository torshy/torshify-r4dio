using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Raven.Client;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;
using System.Threading.Tasks;

namespace Torshify.Radio.EchoNest.Views.Favorites.Tabs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = true)]
    public class FavoritesViewModel : NotificationObject, INavigationAware, IHeaderInfoProvider<HeaderInfo>
    {
        #region Fields

        private ObservableCollection<Favorite> _favoriteList;

        #endregion Fields

        #region Constructors

        public FavoritesViewModel()
        {
            HeaderInfo = new HeaderInfo
            {
                Title = "Favorites"
            };

            PlayFavoriteCommand = new StaticCommand<Favorite>(ExecutePlayFavorite);
            QueueFavoriteCommand = new StaticCommand<Favorite>(ExecuteQueueFavorite);
            DeleteFavoriteCommand = new StaticCommand<Favorite>(ExecuteDeleteFavorite);
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

        public StaticCommand<Favorite> QueueFavoriteCommand
        {
            get;
            private set;
        }

        public StaticCommand<Favorite> DeleteFavoriteCommand
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
        public ILoadingIndicatorService LoadingIndicatorService
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

        public ObservableCollection<Favorite> FavoriteList
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
            Task.Factory.StartNew(() =>
            {
                using (LoadingIndicatorService.EnterLoadingBlock())
                {
                    using (var session = DocumentStore.OpenSession())
                    {
                        FavoriteList = new ObservableCollection<Favorite>(session.Query<Favorite>().ToList());
                    }
                }
            })
            .ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    ToastService.Show("An error occurred during fetching of your favorites");

                    task.Exception.Handle(e =>
                    {
                        Logger.Log(e.ToString(), Category.Exception, Priority.Medium);
                        return true;
                    });
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

        private void ExecuteQueueFavorite(Favorite favorite)
        {
            var favoriteHandler = FavoriteHandlers.FirstOrDefault(f => f.CanHandleFavorite(favorite));

            if (favoriteHandler != null)
            {
                favoriteHandler.Queue(favorite);
            }
            else
            {
                ToastService.Show("Unable to queue favorite");
                Logger.Log("Unable to find favorite handler for type " + favorite.GetType() + " and id " + favorite.Id, Category.Warn, Priority.Medium);
            }
        }

        private void ExecuteDeleteFavorite(Favorite favorite)
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                using (var session = DocumentStore.OpenSession())
                {
                    var favoriteToDelete = session.Load<Favorite>(favorite.Id);
                    session.Delete(favoriteToDelete);
                    session.SaveChanges();
                }
            })
            .ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    ToastService.Show("An error occurred during deletion of your favorite");

                    task.Exception.Handle(e=>
                    {
                        Logger.Log(e.ToString(), Category.Exception, Priority.Medium);
                        return true;
                    });
                }
                else
                {
                    _favoriteList.Remove(favorite);

                    ToastService.Show(new ToastData
                    {
                        Message = "Favorite deleted",
                        Icon = AppIcons.Delete
                    });
                }
            }, ui);
        }

        #endregion Methods
    }
}