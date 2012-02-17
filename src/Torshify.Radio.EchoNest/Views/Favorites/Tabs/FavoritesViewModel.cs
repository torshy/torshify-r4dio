using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Practices.Prism.Commands;
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

        private ObservableCollection<Favorite> _favoriteList;
        private IDocumentSession _session;

        #endregion Fields

        #region Constructors

        public FavoritesViewModel()
        {
            HeaderInfo = new HeaderInfo
            {
                Title = "Favorites"
            };

            CommandBar = new CommandBar();
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

        public ICommandBar CommandBar
        {
            get;
            private set;
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

        public void UpdateCommandBar(IEnumerable<Favorite> selectedFavorites)
        {
            CommandBar.Clear();
            CommandBar
                .AddCommand(new CommandModel
                {
                    Content = "Play",
                    Icon = AppIcons.Play.ToImage(),
                    Command = PlayFavoriteCommand,
                    CommandParameter = selectedFavorites.LastOrDefault()
                })
                .AddCommand(new CommandModel
                {
                    Content = "Queue",
                    Icon = AppIcons.Add.ToImage(),
                    Command = new DelegateCommand<IEnumerable<Favorite>>(favorites => favorites.ForEach(f => QueueFavoriteCommand.Execute(f))),
                    CommandParameter = selectedFavorites.ToArray()
                })
                .AddSeparator()
                .AddCommand(new CommandModel
                {
                    Content = "Remove",
                    Icon = AppIcons.Delete.ToImage(),
                    Command = new DelegateCommand<IEnumerable<Favorite>>(favorites => favorites.ForEach(f => DeleteFavoriteCommand.Execute(f))),
                    CommandParameter = selectedFavorites.ToArray()
                });
        }

        public void MoveItem(Favorite item1, Favorite item2)
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                var tmp = item1.Index;
                item1.Index = item2.Index;
                item2.Index = tmp;
                _session.Store(item1);
                _session.Store(item2);
                _session.SaveChanges();
            })
            .ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    ToastService.Show("An error occurred during moving of your favorite");

                    task.Exception.Handle(e =>
                    {
                        Logger.Log(e.ToString(), Category.Exception, Priority.Medium);
                        return true;
                    });
                }
                else
                {
                    int index1 = _favoriteList.IndexOf(item1);
                    int index2 = _favoriteList.IndexOf(item2);
                    _favoriteList.Move(index1, index2);
                }
            }, ui);
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            Task.Factory.StartNew(() =>
            {
                Logger.Log("Opening db-session", Category.Debug, Priority.Medium);

                _session = DocumentStore.OpenSession();

                using (LoadingIndicatorService.EnterLoadingBlock())
                {
                    FavoriteList = new ObservableCollection<Favorite>(_session.Query<Favorite>().OrderBy(f => f.Index).ToList());
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
            try
            {
                if (_session != null)
                {
                    Logger.Log("Disposing db-session", Category.Debug, Priority.Medium);
                    _session.Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString(), Category.Exception, Priority.Medium);
            }
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

                ToastService.Show(new ToastData
                {
                    Message = "Favorite queued",
                    Icon = AppIcons.Add
                });
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
                _session.Delete(favorite);
                _session.SaveChanges();
            })
            .ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    ToastService.Show("An error occurred during deletion of your favorite");

                    task.Exception.Handle(e =>
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