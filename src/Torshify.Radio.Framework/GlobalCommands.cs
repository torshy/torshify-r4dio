using Microsoft.Practices.Prism.Commands;

namespace Torshify.Radio.Framework
{
    public static class GlobalCommands
    {
        public static readonly CompositeCommand LoadCommand = new CompositeCommand();
        public static readonly CompositeCommand PlayCommand = new CompositeCommand();
        public static readonly CompositeCommand PauseCommand = new CompositeCommand();
        public static readonly CompositeCommand StopCommand = new CompositeCommand();
        public static readonly CompositeCommand NextCommand = new CompositeCommand();
        public static readonly CompositeCommand TogglePlayPauseCommand = new CompositeCommand();
        public static readonly CompositeCommand VolumeUpCommand = new CompositeCommand();
        public static readonly CompositeCommand VolumeDownCommand = new CompositeCommand();
        public static readonly CompositeCommand TuneInStationCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleDebugWindowCommand = new CompositeCommand();
        public static readonly CompositeCommand OpenSettingsCommand = new CompositeCommand();

    }
}