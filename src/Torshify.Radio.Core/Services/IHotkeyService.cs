using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Core.Services
{
    public interface IHotkeyService
    {
        #region Properties

        bool IsEnabled
        {
            get; set;
        }

        IEnumerable<GlobalHotkeyDefinition> AvailableHotkeys
        {
            get;
        }

        IEnumerable<GlobalHotkey> ConfiguredHotkeys
        {
            get;
        }

        #endregion Properties

        #region Methods

        void Add(GlobalHotkey hotkey);

        void Remove(string id);

        void Save();

        void RestoreDefaults();

        #endregion Methods
    }

    public class GlobalHotkey : NotificationObject
    {
        #region Fields

        private GlobalHotkeyDefinition _definition;
        private Keys _keys;

        #endregion Fields

        #region Properties

        public string Id
        {
            get;
            set;
        }

        public GlobalHotkeyDefinition Definition
        {
            get { return _definition; }
            set
            {
                _definition = value;
                RaisePropertyChanged("Definition");
            }
        }

        public Keys Keys
        {
            get { return _keys; }
            set
            {
                _keys = value;
                RaisePropertyChanged("Keys");
            }
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