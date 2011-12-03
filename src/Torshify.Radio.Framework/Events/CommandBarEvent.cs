using Microsoft.Practices.Prism.Events;

namespace Torshify.Radio.Framework.Events
{
    public abstract class CommandBarEvent<T> : CompositePresentationEvent<T>
    {
        #region Constructors

        protected CommandBarEvent(ICommandBar commandBar)
        {
            CommandBar = commandBar;
        }

        #endregion Constructors

        #region Properties

        public ICommandBar CommandBar
        {
            get; private set;
        }

        #endregion Properties
    }
}