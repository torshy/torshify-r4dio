using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Practices.Prism.ViewModel;
using Raven.Client;

using Torshify.Radio.Core.Services;
using Torshify.Radio.Core.Utilities.Hooks;
using Torshify.Radio.Core.Utilities.Hooks.WinApi;
using Torshify.Radio.Framework;

using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace Torshify.Radio.Core.Startables
{
    [Export(typeof(IHotkeyService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GlobalHotkeyHandler : NotificationObject, IStartable, IHotkeyService
    {
        #region Fields

        private const string MuteId = "Mute";
        private const string PlayPauseId = "Play/pause";
        private const string VolumeDownId = "Volume down";
        private const string VolumeUpId = "Volume up";

        private List<GlobalHotkeyDefinition> _availableHotkeys;
        private KeyboardHookListener _globalKeyboardHook;
        private IEnumerable<GlobalHotkey> _hotkeys;

        #endregion Fields

        #region Constructors

        public GlobalHotkeyHandler()
        {
            _availableHotkeys = new List<GlobalHotkeyDefinition>();
            _availableHotkeys.Add(new GlobalHotkeyDefinition(MuteId, "Mute"));
            _availableHotkeys.Add(new GlobalHotkeyDefinition(PlayPauseId, "Play/pause"));
            _availableHotkeys.Add(new GlobalHotkeyDefinition(VolumeDownId, "Volume down"));
            _availableHotkeys.Add(new GlobalHotkeyDefinition(VolumeUpId, "Volume up"));
        }

        #endregion Constructors

        #region Properties

        [Import]
        public IDocumentStore DocumentStore
        {
            get;
            set;
        }

        public IEnumerable<GlobalHotkeyDefinition> AvailableHotkeys
        {
            get { return _availableHotkeys; }
        }

        public IEnumerable<GlobalHotkey> ConfiguredHotkeys
        {
            get { return _hotkeys; }
        }

        #endregion Properties

        #region Methods

        public void Start()
        {
            using (var session = DocumentStore.OpenSession())
            {
                _hotkeys = session.Query<GlobalHotkey>();

                if (_hotkeys == null || !_hotkeys.Any())
                {
                    _hotkeys = GetDefaultHotKeys().ToArray();

                    foreach (var globalHotkey in _hotkeys)
                    {
                        session.Store(globalHotkey);
                    }

                    session.SaveChanges();
                }
            }

            _globalKeyboardHook = new KeyboardHookListener(new GlobalHooker());
            _globalKeyboardHook.KeyDown += GlobalKeyboardHookOnKeyDown;
            _globalKeyboardHook.Enabled = true;

            RaisePropertyChanged("ConfiguredHotkeys", "AvailableHotkeys");
        }

        private IEnumerable<GlobalHotkey> GetDefaultHotKeys()
        {
            yield return new GlobalHotkey
            {
                Definition = AvailableHotkeys.FirstOrDefault(d => d.DefinitionId == PlayPauseId),
                Keys = Keys.MediaPlayPause
            };

            yield return new GlobalHotkey
            {
                Definition = AvailableHotkeys.FirstOrDefault(d => d.DefinitionId == VolumeUpId),
                Keys = Keys.Up | Keys.Control
            };

            yield return new GlobalHotkey
            {
                Definition = AvailableHotkeys.FirstOrDefault(d => d.DefinitionId == VolumeDownId),
                Keys = Keys.Down | Keys.Control
            };

            yield return new GlobalHotkey
            {
                Definition = AvailableHotkeys.FirstOrDefault(d => d.DefinitionId == MuteId),
                Keys = Keys.VolumeMute
            };
        }

        private void GlobalKeyboardHookOnKeyDown(object sender, KeyEventArgs e)
        {
            var hotKey = _hotkeys.FirstOrDefault(h => h.Keys == (e.KeyCode | e.Modifiers));

            if (hotKey != null)
            {
                switch (hotKey.Definition.DefinitionId)
                {
                    case PlayPauseId:
                        AppCommands.TogglePlayCommand.Execute(null);
                        break;
                    case VolumeUpId:
                        AppCommands.IncreaseVolumeCommand.Execute(null);
                        break;
                    case VolumeDownId:
                        AppCommands.DecreaseVolumeCommand.Execute(null);
                        break;
                    case MuteId:
                        AppCommands.ToggleMuteCommand.Execute(null);
                        break;
                }
            }
        }

        #endregion Methods
    }
}