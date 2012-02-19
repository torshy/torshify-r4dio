using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;

using Raven.Client;

using Torshify.Radio.Core.Models;
using Torshify.Radio.Core.Services;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.Core.Views.Settings.Tabs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralSettingsViewModel : NotificationObject, IHeaderInfoProvider<HeaderInfo>
    {
        #region Fields

        private Color _currentAccentColor;

        #endregion Fields

        #region Constructors

        public GeneralSettingsViewModel()
        {
            HeaderInfo = new HeaderInfo
            {
                Title = "General",
                IconUri = AppIcons.Settings.ToString()
            };

            _currentAccentColor = (Color)Application.Current.TryFindResource(AppTheme.AccentColorKey);
            AddHotkeyCommand = new StaticCommand(ExecuteAddHotkey);
            RemoveHotkeyCommand = new AutomaticCommand<GlobalHotkey>(ExecuteRemoveHotkey, CanExecuteRemoveHotkey);
            RestoreDefaultHotkeysCommand = new StaticCommand(ExecuteRestoreDefaultHotkeys);
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
        public IHotkeyService HotkeyService
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

        [Import]
        public ILoggerFacade Logger
        {
            get;
            set;
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

        public HeaderInfo HeaderInfo
        {
            get;
            private set;
        }

        public Color CurrentAccentColor
        {
            get
            {
                return _currentAccentColor;
            }
            set
            {
                if (_currentAccentColor != value)
                {
                    _currentAccentColor = value;

                    Application.Current.Resources[AppTheme.AccentColorKey] = value;
                    Application.Current.Resources[AppTheme.AccentBrushKey] = new SolidColorBrush(value);

                    RaisePropertyChanged("CurrentAccentColor");
                }
            }
        }

        #endregion Properties

        #region Methods

        public void Save()
        {
            try
            {
                using (var session = DocumentStore.OpenSession())
                {
                    var settings = session.Query<ApplicationSettings>().FirstOrDefault();

                    if (settings != null)
                    {
                        settings.AccentColor = CurrentAccentColor;
                    }

                    session.Store(settings);
                    session.SaveChanges();
                }

                HotkeyService.Save();
            }
            catch (Exception e)
            {
                ToastService.Show(new ToastData
                {
                    Message = "Error while saving general settings"
                });

                Logger.Log(e.ToString(), Category.Exception, Priority.Medium);
            }
        }

        private void ExecuteRestoreDefaultHotkeys()
        {
            HotkeyService.RestoreDefaults();
        }

        private bool CanExecuteRemoveHotkey(GlobalHotkey hotkey)
        {
            return hotkey != null && HotkeyService.ConfiguredHotkeys.Contains(hotkey);
        }

        private void ExecuteRemoveHotkey(GlobalHotkey hotkey)
        {
            HotkeyService.Remove(hotkey.Id);
        }

        private void ExecuteAddHotkey()
        {
            HotkeyService.Add(new GlobalHotkey());
        }

        #endregion Methods
    }
}