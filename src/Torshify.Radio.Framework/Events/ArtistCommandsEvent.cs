using Microsoft.Practices.Prism.Events;

namespace Torshify.Radio.Framework.Events
{
    public class ArtistCommandsEvent : CompositePresentationEvent<ArtistCommandsPayload>
    {
    }

    public class ArtistCommandsPayload
    {
        #region Constructors

        public ArtistCommandsPayload(string artistName)
        {
            ArtistName = artistName;
            CommandBar = new CommandBar();
        }

        #endregion Constructors

        #region Properties

        public string ArtistName
        {
            get; private set;
        }

        public ICommandBar CommandBar
        {
            get; private set;
        }

        #endregion Properties
    }
}