using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
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

        private StaticCommand<object> _addTrackContainerToFavoritesCommand;
        private StaticCommand<object> _addTrackContainerToLikesCommand;
        private StaticCommand<object> _addTrackToFavoritesCommand;
        private StaticCommand<object> _addTrackToLikesCommand;

        #endregion Fields

        #region Constructors

        public AppCommandsHandler()
        {
            _addTrackToFavoritesCommand = new StaticCommand<object>(ExecuteAddTrackToFavorites);
            _addTrackContainerToFavoritesCommand = new StaticCommand<object>(ExecuteAddTrackContainerToFavorites);
            _addTrackToLikesCommand = new StaticCommand<object>(ExecuteAddTrackToLikes);
            _addTrackContainerToLikesCommand = new StaticCommand<object>(ExecuteAddTrackContainerToLikes);
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

            AppCommands.AddToFavoriteCommand.RegisterCommand(_addTrackToFavoritesCommand);
            AppCommands.AddToFavoriteCommand.RegisterCommand(_addTrackContainerToFavoritesCommand);
            AppCommands.AddToLikeCommand.RegisterCommand(_addTrackToLikesCommand);
            AppCommands.AddToLikeCommand.RegisterCommand(_addTrackContainerToLikesCommand);

            AppCommands.AddTrackContainerToFavoriteCommand.RegisterCommand(_addTrackContainerToFavoritesCommand);
            AppCommands.AddTrackContainerToLikesCommand.RegisterCommand(_addTrackContainerToLikesCommand);
            AppCommands.AddTrackToLikeCommand.RegisterCommand(_addTrackToLikesCommand);
            AppCommands.AddTrackToFavoriteCommand.RegisterCommand(_addTrackToFavoritesCommand);

            Application.Current.MainWindow.InputBindings.Add(
                new MouseBinding(
                    AppCommands.NavigateBackCommand,
                     new ExtendedMouseGesture(MouseButton.XButton1)));

            Application.Current.MainWindow.InputBindings.Add(
                new MouseBinding(
                    AppCommands.NavigateForwardCommand,
                    new ExtendedMouseGesture(MouseButton.XButton2)));
        }

        private void ExecuteAddTrackContainerToLikes(object parameter)
        {
            TrackContainer container = parameter as TrackContainer;

            if (container != null)
            {

            }
        }

        private void ExecuteAddTrackToLikes(object parameter)
        {
            Track track = parameter as Track;

            if (track != null)
            {

            }
        }

        private void ExecuteAddTrackContainerToFavorites(object parameter)
        {
            TrackContainer container = parameter as TrackContainer;

            if (container != null)
            {
                using (var session = DocumentStore.OpenSession())
                {
                    TrackContainerFavorite fav = new TrackContainerFavorite();
                    fav.TrackContainer = container;
                    session.Store((Favorite)fav);
                    session.SaveChanges();
                }
            }
        }

        private void ExecuteAddTrackToFavorites(object parameter)
        {
            Track track = parameter as Track;

            if (track != null)
            {
                using (var session = DocumentStore.OpenSession())
                {
                    TrackFavorite fav = new TrackFavorite();
                    fav.Track = track;
                    session.Store((Favorite)fav);
                    session.SaveChanges();
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


    public class Favorite
    {
        public string Id
        {
            get;
            set;
        }
    }

    public class TrackFavorite : Favorite
    {
        public Track Track
        {
            get;
            set;
        }
    }

    public class TrackContainerFavorite : Favorite
    {
        public TrackContainer TrackContainer
        {
            get;
            set;
        }
    }
}