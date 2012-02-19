using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using Raven.Client;
using Torshify.Radio.Core.Models;
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
        private ObservableCollection<GlobalHotkey> _hotkeys;

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

        [Import]
        public ILoggerFacade Logger
        {
            get;
            set;
        }

        [Import]
        public IToastService ToastService
        {
            get;
            set;
        }

        public bool IsEnabled
        {
            get
            {
                if (_globalKeyboardHook != null)
                {
                    return _globalKeyboardHook.Enabled;
                }

                return true;
            }
            set
            {
                try
                {
                    using (var session = DocumentStore.OpenSession())
                    {
                        var settings = session.Query<ApplicationSettings>().FirstOrDefault();

                        if (settings != null)
                        {
                            settings.HotkeysEnabled = value;
                            session.Store(settings);
                            session.SaveChanges();
                        }
                    }

                    _globalKeyboardHook.Enabled = value;
                }
                catch (Exception e)
                {
                    ToastService.Show("Error while enabling/disabling hotkeys");
                    Logger.Log(e.ToString(), Category.Exception, Priority.Medium);
                }

                RaisePropertyChanged("IsEnabled");
            }
        }

        public IEnumerable<GlobalHotkeyDefinition> AvailableHotkeys
        {
            get
            {
                return _availableHotkeys;
            }
        }

        public IEnumerable<GlobalHotkey> ConfiguredHotkeys
        {
            get
            {
                return _hotkeys;
            }
        }

        public void Add(GlobalHotkey hotkey)
        {
            try
            {
                using (var session = DocumentStore.OpenSession())
                {
                    session.Store(hotkey);
                    session.SaveChanges();
                }

                _hotkeys.Add(hotkey);
            }
            catch (Exception e)
            {
                ToastService.Show(new ToastData
                {
                    Message = "Unexpected error while adding hotkey. " + e.Message
                });

                Logger.Log(e.ToString(), Category.Exception, Priority.Medium);
            }
        }

        public void Remove(string id)
        {
            try
            {
                using (var session = DocumentStore.OpenSession())
                {
                    var hotkey = session.Load<GlobalHotkey>(id);
                    if (hotkey != null)
                    {
                        session.Delete(hotkey);
                        session.SaveChanges();
                    }
                }

                _hotkeys
                    .Where(h => h.Id == id)
                    .ToArray()
                    .ForEach(h => _hotkeys.Remove(h));
            }
            catch (Exception e)
            {
                ToastService.Show(new ToastData
                {
                    Message = "Unexpected error while removing hotkey. " + e.Message
                });

                Logger.Log(e.ToString(), Category.Exception, Priority.Medium);
            }
        }

        public void Save()
        {
            if (_hotkeys == null)
            {
                return;
            }

            try
            {
                using (var session = DocumentStore.OpenSession())
                {
                    foreach (var configuredHotkey in ConfiguredHotkeys)
                    {
                        session.Store(configuredHotkey);
                    }

                    session.SaveChanges();
                }
            }
            catch (Exception e)
            {
                ToastService.Show(new ToastData
                {
                    Message = "Unexpected error while removing hotkey. " + e.Message
                });

                Logger.Log(e.ToString(), Category.Exception, Priority.Medium);
            }
        }

        public void RestoreDefaults()
        {
            try
            {
                using (var session = DocumentStore.OpenSession())
                {
                    var hotkeys = session.Query<GlobalHotkey>();

                    if (hotkeys == null || !hotkeys.Any())
                    {
                        _hotkeys = GetDefaultHotKeys().ToObservableCollection();

                        foreach (var globalHotkey in _hotkeys)
                        {
                            session.Store(globalHotkey);
                        }
                    }
                    else
                    {
                        foreach (var globalHotkey in hotkeys)
                        {
                            session.Delete(globalHotkey);
                        }

                        _hotkeys = GetDefaultHotKeys().ToObservableCollection();

                        foreach (var globalHotkey in _hotkeys)
                        {
                            session.Store(globalHotkey);
                        }
                    }

                    session.SaveChanges();
                }
        
            }
            catch (Exception e)
            {
                ToastService.Show("Error occurred while restoring hotkey defaults");
                Logger.Log(e.ToString(), Category.Exception, Priority.Medium);
            }

            RaisePropertyChanged(string.Empty);
        }

        #endregion Properties

        #region Methods

        public void Start()
        {
            using (var session = DocumentStore.OpenSession())
            {
                var hotkeys = session.Query<GlobalHotkey>();

                if (hotkeys == null || !hotkeys.Any())
                {
                    _hotkeys = GetDefaultHotKeys().ToObservableCollection();

                    foreach (var globalHotkey in _hotkeys)
                    {
                        session.Store(globalHotkey);
                    }

                    session.SaveChanges();
                }
                else
                {
                    _hotkeys = hotkeys.ToObservableCollection();
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