using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;

using Raven.Client;

using Torshify.Radio.Core.Models;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Views.Settings.General
{
    [Export("GeneralSettingsSection", typeof(ISettingsSection))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ThemeSection : NotificationObject, ISettingsSection
    {
        #region Fields

        private Color _currentAccentColor;
        [Import]
        private IDocumentStore _documentStore = null;
        [Import]
        private ILoggerFacade _logger = null;
        [Import]
        private IToastService _toastService = null;

        #endregion Fields

        #region Constructors

        public ThemeSection()
        {
            HeaderInfo = new HeaderInfo
            {
                Title = "Theme"
            };

            UI = new ThemeSectionView
            {
                DataContext = this
            };

            _currentAccentColor = (Color)Application.Current.TryFindResource(AppTheme.AccentColorKey);
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
                    CoreModule.ModifyAccentColor(value);
                    RaisePropertyChanged("CurrentAccentColor");
                }
            }
        }

        #endregion Properties

        #region Methods

        public void Load()
        {
        }

        public void Save()
        {
            try
            {
                using (var session = _documentStore.OpenSession())
                {
                    var settings = session.Query<ApplicationSettings>().FirstOrDefault();

                    if (settings != null)
                    {
                        settings.AccentColor = CurrentAccentColor;
                        session.Store(settings);
                        session.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                _toastService.Show(new ToastData
                {
                    Message = "Error while saving general settings"
                });

                _logger.Log(e.ToString(), Category.Exception, Priority.Medium);
            }
        }

        #endregion Methods
    }
}