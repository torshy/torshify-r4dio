using Microsoft.Practices.Prism.Commands;

namespace Torshify.Radio.Framework
{
    public class AppCommands
    {
        public static readonly CompositeCommand PlayTracksCommand = new CompositeCommand();
        public static readonly CompositeCommand QueueTracksCommand = new CompositeCommand();
        public static readonly CompositeCommand TogglePlayCommand = new CompositeCommand();
        public static readonly CompositeCommand NextTrackCommand = new CompositeCommand();
        public static readonly CompositeCommand IncreaseVolumeCommand = new CompositeCommand();
        public static readonly CompositeCommand DecreaseVolumeCommand = new CompositeCommand();
        public static readonly CompositeCommand ToggleMuteCommand = new CompositeCommand();

        public static readonly CompositeCommand NavigateBackCommand = new CompositeCommand();
        public static readonly CompositeCommand NavigateForwardCommand = new CompositeCommand();

        public static readonly CompositeCommand AddToFavoriteCommand = new CompositeCommand();
        public static readonly CompositeCommand AddTrackToFavoriteCommand = new CompositeCommand();
        public static readonly CompositeCommand AddTrackContainerToFavoriteCommand = new CompositeCommand();
        public static readonly CompositeCommand AddTrackStreamToFavoriteCommand = new CompositeCommand();
        public static readonly CompositeCommand AddTrackStreamDataToFavoriteCommand = new CompositeCommand();

        public static readonly CompositeCommand LikeTrackCommand = new CompositeCommand();

        public static readonly CompositeCommand GoToArtistCommand = new CompositeCommand();
        public static readonly CompositeCommand GoToAlbumCommand = new CompositeCommand();
    }
}