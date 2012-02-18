using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using Microsoft.Practices.Prism.ViewModel;

using Raven.Client;

using Torshify.Radio.Core.Models;
using Torshify.Radio.Core.Services;
using Torshify.Radio.Framework;

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
        }

        #endregion Constructors

        #region Properties

        [Import]
        public IDocumentStore DocumentStore
        {
            get; set;
        }

        [Import]
        public IHotkeyService HotkeyService
        {
            get; 
            set;
        }

        public HeaderInfo HeaderInfo
        {
            get;
            private set;
        }

        public Color CurrentAccentColor
        {
            get { return _currentAccentColor; }
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
        }

        #endregion Methods
    }
}