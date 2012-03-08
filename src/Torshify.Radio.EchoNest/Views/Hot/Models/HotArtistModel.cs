using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest.Views.Hot.Models
{
    public class HotArtistModel : NotificationObject
    {
        public HotArtistModel(int index, string name, string artistImage, string hotSongTitle)
        {
            Index = index;
            Name = name;
            HotSongTitle = hotSongTitle;
            ArtistImage = artistImage;
        }

        public int Index
        {
            get; 
            private set;
        }

        public string Name
        {
            get; 
            private set;
        }

        public string HotSongTitle
        {
            get; 
            private set; 
        }

        public string ArtistImage
        {
            get; 
            private set;
        }
    }
}