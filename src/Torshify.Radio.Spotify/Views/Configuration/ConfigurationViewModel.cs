using System.ComponentModel.Composition;
using System.Windows;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Spotify.Views.Configuration
{
    [ConfigurationMetadata(Name = "Spotify", Icon = "pack://siteoforigin:,,,/Resources/Icons/Spotify_Logo.png")]
    public class ConfigurationViewModel : NotificationObject, IConfiguration
    {
        #region Constructors

        [ImportingConstructor]
        public ConfigurationViewModel(ConfigurationView view)
        {
            UI = view;
            UI.DataContext = this;
        }

        #endregion Constructors

        #region Properties

        public FrameworkElement UI
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public void Commit()
        {
        }

        public void Cancel()
        {
        }

        #endregion Methods
    }
}