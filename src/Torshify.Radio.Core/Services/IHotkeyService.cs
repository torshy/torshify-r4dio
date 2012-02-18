using System.Collections.Generic;
using System.Windows.Forms;

namespace Torshify.Radio.Core.Services
{
    public interface IHotkeyService
    {
        #region Properties

        IEnumerable<GlobalHotkeyDefinition> AvailableHotkeys
        {
            get;
        }

        IEnumerable<GlobalHotkey> ConfiguredHotkeys
        {
            get;
        }

        #endregion Properties
    }

    public class GlobalHotkey
    {
        #region Properties

        public string Id
        {
            get;
            set;
        }

        public GlobalHotkeyDefinition Definition
        {
            get;
            set;
        }

        public Keys Keys
        {
            get;
            set;
        }

        #endregion Properties
    }

    public class GlobalHotkeyDefinition
    {
        #region Constructors

        public GlobalHotkeyDefinition(string id, string description)
        {
            DefinitionId = id;
            Description = description;
        }

        #endregion Constructors

        #region Properties

        public string DefinitionId
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public static bool operator ==(GlobalHotkeyDefinition left, GlobalHotkeyDefinition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GlobalHotkeyDefinition left, GlobalHotkeyDefinition right)
        {
            return !Equals(left, right);
        }

        public bool Equals(GlobalHotkeyDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.DefinitionId, DefinitionId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GlobalHotkeyDefinition)) return false;
            return Equals((GlobalHotkeyDefinition) obj);
        }

        public override int GetHashCode()
        {
            return (DefinitionId != null ? DefinitionId.GetHashCode() : 0);
        }

        #endregion Methods
    }
}