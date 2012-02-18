using Microsoft.Practices.Prism.Events;

namespace Torshify.Radio.Framework.Events
{
    public class AlbumRelatedCommandBarPayload
    {
        #region Constructors

        public AlbumRelatedCommandBarPayload(string artistName, string albumName, ICommandBar commandBar)
        {
            ArtistName = artistName;
            AlbumName = albumName;
            CommandBar = commandBar;
        }

        #endregion Constructors

        #region Properties

        public string ArtistName
        {
            get;
            private set;
        }

        public string AlbumName
        {
            get;
            private set;
        }

        public ICommandBar CommandBar
        {
            get;
            private set;
        }

        #endregion Properties
    }

    public class BuildAlbumRelatedCommandBarEvent : CompositePresentationEvent<AlbumRelatedCommandBarPayload>
    {
    }
}