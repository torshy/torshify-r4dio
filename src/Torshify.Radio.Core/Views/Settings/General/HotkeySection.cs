using System.ComponentModel.Composition;
using System.Linq;

using Torshify.Radio.Core.Services;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.Core.Views.Settings.General
{
    [Export("GeneralSettingsSection", typeof(ISettingsSection))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HotkeySection : ISettingsSection
    {
        [Import]
        private IHotkeyService _hotkeyService;

        #region Constructors

        public HotkeySection()
        {
            HeaderInfo = new HeaderInfo
            {
                Title = "Global hotkeys"
            };

            UI = new HotkeySectionView
            {
                DataContext = this
            };

            AddHotkeyCommand = new StaticCommand(ExecuteAddHotkey);
            RemoveHotkeyCommand = new AutomaticCommand<GlobalHotkey>(ExecuteRemoveHotkey, CanExecuteRemoveHotkey);
            RestoreDefaultHotkeysCommand = new StaticCommand(ExecuteRestoreDefaultHotkeys);
        }

        #endregion Constructors

        #region Properties

        public HeaderInfo HeaderInfo
        {
            get;
            private set;
        }

        public object UI
        {
            get;
            private set;
        }

        public IHotkeyService HotkeyService
        {
            get
            {
                return _hotkeyService;
            }
        }

        public StaticCommand RestoreDefaultHotkeysCommand
        {
            get;
            private set;
        }

        public StaticCommand AddHotkeyCommand
        {
            get;
            private set;
        }

        public AutomaticCommand<GlobalHotkey> RemoveHotkeyCommand
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public void Load()
        {
        }

        public void Save()
        {
            _hotkeyService.Save();
        }

        private void ExecuteRestoreDefaultHotkeys()
        {
            _hotkeyService.RestoreDefaults();
        }

        private bool CanExecuteRemoveHotkey(GlobalHotkey hotkey)
        {
            return hotkey != null && _hotkeyService.ConfiguredHotkeys.Contains(hotkey);
        }

        private void ExecuteRemoveHotkey(GlobalHotkey hotkey)
        {
            _hotkeyService.Remove(hotkey.Id);
        }

        private void ExecuteAddHotkey()
        {
            _hotkeyService.Add(new GlobalHotkey());
        }
        #endregion Methods
    }
}