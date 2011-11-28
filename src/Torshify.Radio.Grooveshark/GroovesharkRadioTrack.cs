using Torshify.Radio.Framework;

namespace Torshify.Radio.Grooveshark
{
    public class GroovesharkRadioTrack : RadioTrack
    {
        public int SongID
        {
            get; 
            set;
        }

        public int AlbumID
        {
            get; 
            set;
        }

        public int ArtistID
        {
            get; 
            set;
        }
    }
}