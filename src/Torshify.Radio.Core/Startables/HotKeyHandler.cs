using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;

using Raven.Client;

using Torshify.Radio.Core.Utilities.Hooks;
using Torshify.Radio.Core.Utilities.Hooks.WinApi;
using Torshify.Radio.Framework;

using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace Torshify.Radio.Core.Startables
{
    public class GlobalHotKey
    {
        #region Properties

        public string Id
        {
            get; set;
        }

        public Keys Keys
        {
            get; set;
        }

        #endregion Properties
    }

    public class GlobalHotKeySettings
    {
        #region Constructors

        public GlobalHotKeySettings()
        {
            HotKeys = new List<GlobalHotKey>();
        }

        #endregion Constructors

        #region Properties

        public List<GlobalHotKey> HotKeys
        {
            get; set;
        }

        #endregion Properties
    }

    public class HotKeyHandler : IStartable
    {
        #region Fields

        private const string MuteId = "Mute";
        private const string PlayPauseId = "Play/pause";
        private const string VolumeDownId = "Volume down";
        private const string VolumeUpId = "Volume up";

        private KeyboardHookListener _globalKeyboardHook;
        private GlobalHotKeySettings _settings;

        #endregion Fields

        #region Properties

        [Import]
        public IDocumentStore DocumentStore
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void Start()
        {
            using(var session = DocumentStore.OpenSession())
            {
                _settings = session.Query<GlobalHotKeySettings>().FirstOrDefault();

                if (_settings == null)
                {
                    _settings = GetDefaultHotKeys();
                    session.Store(_settings);
                    session.SaveChanges();
                }
            }

            _globalKeyboardHook = new KeyboardHookListener(new GlobalHooker());
            _globalKeyboardHook.KeyDown += GlobalKeyboardHookOnKeyDown;
            _globalKeyboardHook.Enabled = true;
        }

        private GlobalHotKeySettings GetDefaultHotKeys()
        {
            GlobalHotKeySettings settings = new GlobalHotKeySettings();
            settings.HotKeys.Add(new GlobalHotKey
            {
                Id = PlayPauseId,
                Keys = Keys.MediaPlayPause
            });
            settings.HotKeys.Add(new GlobalHotKey
            {
                Id = VolumeUpId,
                Keys = Keys.Up | Keys.Control
            });
            settings.HotKeys.Add(new GlobalHotKey
            {
                Id = VolumeDownId,
                Keys = Keys.Down | Keys.Control
            });
            settings.HotKeys.Add(new GlobalHotKey
            {
                Id = MuteId,
                Keys = Keys.VolumeMute
            });

            return settings;
        }

        private void GlobalKeyboardHookOnKeyDown(object sender, KeyEventArgs e)
        {
            var hotKey = _settings.HotKeys.FirstOrDefault(h => h.Keys == (e.KeyCode | e.Modifiers));
            
            if (hotKey != null)
            {
                switch(hotKey.Id)
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