using System.ComponentModel.Composition;
using System.Windows;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Views.Configuration
{
    [ConfigurationMetadata(Name = "General", Icon = "pack://application:,,,/Resources/SmallIcons/settings_gray.png")]
    public class GeneralSettingsViewModel : NotificationObject, IConfiguration
    {
        #region Constructors

        [ImportingConstructor]
        public GeneralSettingsViewModel(GeneralSettingsView view)
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

        public void Cancel()
        {
        }

        public void Commit()
        {
        }

        #endregion Methods
    }
}