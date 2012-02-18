using Microsoft.Practices.Prism.Events;

namespace Torshify.Radio.Framework.Events
{
    public class ArtistRelatedCommandBarPayload
    {
        #region Constructors

        public ArtistRelatedCommandBarPayload(string artistName, ICommandBar commandBar)
        {
            ArtistName = artistName;
            CommandBar = commandBar;
        }

        #endregion Constructors

        #region Properties

        public string ArtistName
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

    public class BuildArtistRelatedCommandBarEvent : CompositePresentationEvent<ArtistRelatedCommandBarPayload>
    {
    }
}