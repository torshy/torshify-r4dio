using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;

using Raven.Client;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;
using Torshify.Radio.Framework.Input;

namespace Torshify.Radio.Core.Startables
{
    public class AppCommandsHandler : IStartable
    {
        #region Fields

        private readonly StaticCommand<object> _addTrackContainerToFavoritesCommand;
        private readonly StaticCommand<object> _addTrackStreamDataToFavoritesCommand;
        private readonly StaticCommand<object> _addTrackStreamToFavoritesCommand;
        private readonly StaticCommand<object> _addTrackToFavoritesCommand;
        private readonly StaticCommand<object> _addTrackToLikesCommand;

        #endregion Fields

        #region Constructors

        public AppCommandsHandler()
        {
            _addTrackStreamDataToFavoritesCommand = new StaticCommand<object>(ExecuteAddTrackStreamDataToFavorites);
            _addTrackStreamToFavoritesCommand = new StaticCommand<object>(ExecuteAddTrackStreamToFavorites);
            _addTrackToFavoritesCommand = new StaticCommand<object>(ExecuteAddTrackToFavorites);
            _addTrackContainerToFavoritesCommand = new StaticCommand<object>(ExecuteAddTrackContainerToFavorites);
            _addTrackToLikesCommand = new StaticCommand<object>(ExecuteAddTrackToLikes);
        }

        #endregion Constructors

        #region Properties

        [Import]
        public IRadio Radio
        {
            get;
            set;
        }

        [Import("CorePlayer")]
        public ITrackPlayer Player
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

        #endregion Properties

        #region Methods

        public void Start()
        {
            AppCommands.IncreaseVolumeCommand.RegisterCommand(new DelegateCommand(ExecuteIncreaseVolume));
            AppCommands.DecreaseVolumeCommand.RegisterCommand(new DelegateCommand(ExecuteDecreaseVolume));

            AppCommands.TogglePlayCommand.RegisterCommand(new AutomaticCommand(ExecuteTogglePlay, CanTogglePlay));
            AppCommands.ToggleMuteCommand.RegisterCommand(new DelegateCommand(ExecuteToggleMute));

            AppCommands.NavigateBackCommand.RegisterCommand(new AutomaticCommand(ExecuteNavigateBack, CanNavigateBack));
            AppCommands.NavigateForwardCommand.RegisterCommand(new AutomaticCommand(ExecuteNavigateForward, CanNavigateForward));

            AppCommands.PlayTracksCommand.RegisterCommand(new StaticCommand<object>(ExecutePlayTrackStream));
            AppCommands.QueueTracksCommand.RegisterCommand(new StaticCommand<object>(ExecuteQueueTrackStream));

            AppCommands.AddToFavoriteCommand.RegisterCommand(_addTrackToFavoritesCommand);
            AppCommands.AddToFavoriteCommand.RegisterCommand(_addTrackContainerToFavoritesCommand);
            AppCommands.AddToFavoriteCommand.RegisterCommand(_addTrackStreamToFavoritesCommand);
            AppCommands.AddToFavoriteCommand.RegisterCommand(_addTrackStreamDataToFavoritesCommand);
            AppCommands.AddTrackContainerToFavoriteCommand.RegisterCommand(_addTrackContainerToFavoritesCommand);
            AppCommands.AddTrackToFavoriteCommand.RegisterCommand(_addTrackToFavoritesCommand);
            AppCommands.AddTrackStreamToFavoriteCommand.RegisterCommand(_addTrackStreamToFavoritesCommand);
            AppCommands.AddTrackStreamDataToFavoriteCommand.RegisterCommand(_addTrackStreamDataToFavoritesCommand);

            AppCommands.LikeTrackCommand.RegisterCommand(_addTrackToLikesCommand);

            Application.Current.MainWindow.InputBindings.Add(
                new MouseBinding(
                    AppCommands.NavigateBackCommand,
                     new ExtendedMouseGesture(MouseButton.XButton1)));

            Application.Current.MainWindow.InputBindings.Add(
                new MouseBinding(
                    AppCommands.NavigateForwardCommand,
                    new ExtendedMouseGesture(MouseButton.XButton2)));
        }

        private void ExecuteQueueTrackStream(object parameter)
        {
            ITrackStream trackStream = parameter as ITrackStream;

            if (trackStream != null)
            {
                Radio.Queue(trackStream);
            }
        }

        private void ExecutePlayTrackStream(object parameter)
        {
            ITrackStream trackStream = parameter as ITrackStream;

            if (trackStream != null)
            {
                Radio.Play(trackStream);
            }
        }

        private void ExecuteAddTrackToLikes(object parameter)
        {
            Track track = parameter as Track;

            if (track != null)
            {
                ToastService.Show("Not implemented");
            }
        }

        private void ExecuteAddTrackStreamToFavorites(object parameter)
        {
            ITrackStream trackStream = parameter as ITrackStream;

            if (trackStream != null)
            {
                ExecuteAddTrackStreamDataToFavorites(trackStream.Data);
            }
        }

        private void ExecuteAddTrackStreamDataToFavorites(object parameter)
        {
            TrackStreamData streamData = parameter as TrackStreamData;

            if (streamData != null)
            {
                try
                {
                    using (var session = DocumentStore.OpenSession())
                    {
                        TrackStreamFavorite fav = new TrackStreamFavorite();
                        fav.StreamData = streamData;
                        session.Store(fav);
                        session.SaveChanges();
                    }

                    ToastService.Show(new ToastData
                    {
                        Icon = AppIcons.Save,
                        Message = "Favorite added"
                    });
                }
                catch (Exception e)
                {
                    ToastService.Show("Unable to save favorite. " + e.Message);
                    Logger.Log("Error while saving favorite. " + e, Category.Exception, Priority.High);
                }
            }
        }

        private void ExecuteAddTrackContainerToFavorites(object parameter)
        {
            TrackContainer container = parameter as TrackContainer;

            if (container != null)
            {
                try
                {
                    using (var session = DocumentStore.OpenSession())
                    {
                        TrackContainerFavorite fav = new TrackContainerFavorite();
                        fav.TrackContainer = container;
                        session.Store(fav);
                        session.SaveChanges();
                    }

                    ToastService.Show(new ToastData
                    {
                        Icon = AppIcons.Save,
                        Message = "Favorite added"
                    });
                }
                catch (Exception e)
                {
                    ToastService.Show("Unable to save favorite. " + e.Message);
                    Logger.Log("Error while saving favorite. " + e, Category.Exception, Priority.High);
                }
            }
        }

        private void ExecuteAddTrackToFavorites(object parameter)
        {
            Track track = parameter as Track;

            if (track != null)
            {
                try
                {
                    using (var session = DocumentStore.OpenSession())
                    {
                        TrackFavorite fav = new TrackFavorite();
                        fav.Track = track;
                        session.Store(fav);
                        session.SaveChanges();
                    }

                    ToastService.Show(new ToastData
                    {
                        Icon = AppIcons.Save,
                        Message = "Favorite added"
                    });
                }
                catch (Exception e)
                {
                    ToastService.Show("Unable to save favorite. " + e.Message);
                    Logger.Log("Error while saving favorite. " + e, Category.Exception, Priority.High);
                }
            }
        }

        private void ExecuteToggleMute()
        {
            Player.IsMuted = !Player.IsMuted;
        }

        private bool CanTogglePlay()
        {
            return Radio.CurrentTrack != null;
        }

        private void ExecuteTogglePlay()
        {
            if (Player.IsPlaying)
            {
                Player.Pause();
            }
            else
            {
                Player.Play();
            }
        }

        private bool CanNavigateForward()
        {
            if (RegionManager.Regions.ContainsRegionWithName(AppRegions.ViewRegion))
            {
                return RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.CanGoForward;
            }

            return false;
        }

        private void ExecuteNavigateForward()
        {
            RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.GoForward();
        }

        private bool CanNavigateBack()
        {
            if (RegionManager.Regions.ContainsRegionWithName(AppRegions.ViewRegion))
            {
                return RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.CanGoBack;
            }

            return false;
        }

        private void ExecuteNavigateBack()
        {
            RegionManager.Regions[AppRegions.ViewRegion].NavigationService.Journal.GoBack();
        }

        private void ExecuteDecreaseVolume()
        {
            Player.Volume = Math.Max(0.0f, Player.Volume - 0.01f);
        }

        private void ExecuteIncreaseVolume()
        {
            Player.Volume = Math.Min(1.0f, Player.Volume + 0.01f);
        }

        #endregion Methods
    }
}