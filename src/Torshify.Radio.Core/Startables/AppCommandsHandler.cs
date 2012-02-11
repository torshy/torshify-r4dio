using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;
using Torshify.Radio.Framework.Input;

namespace Torshify.Radio.Core.Startables
{
    public class AppCommandsHandler : IStartable
    {
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

            Application.Current.MainWindow.InputBindings.Add(
                new MouseBinding(
                    AppCommands.NavigateBackCommand,
                     new ExtendedMouseGesture(MouseButton.XButton1)));

            Application.Current.MainWindow.InputBindings.Add(
                new MouseBinding(
                    AppCommands.NavigateForwardCommand,
                    new ExtendedMouseGesture(MouseButton.XButton2)));
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
            Player.Volume = Math.Max(0.0f, Player.Volume - 0.1f);
        }

        private void ExecuteIncreaseVolume()
        {
            Player.Volume = Math.Min(1.0f, Player.Volume + 0.1f);
        }

        #endregion Methods
    }
}