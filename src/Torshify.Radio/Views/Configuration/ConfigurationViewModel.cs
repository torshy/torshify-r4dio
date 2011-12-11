using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Views.Configuration
{
    [Export(typeof(ConfigurationViewModel))]
    public class ConfigurationViewModel : NotificationObject, IPartImportsSatisfiedNotification
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        [ImportMany]
        private IEnumerable<Lazy<IConfiguration, IConfigurationMetadata>> _configurations = null;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public ConfigurationViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            ApplyCommand = new DelegateCommand(ExecuteApply);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        #endregion Constructors

        #region Properties

        public ICommand ApplyCommand
        {
            get; private set;
        }

        public ICommand CancelCommand
        {
            get; private set;
        }

        public IEnumerable<Lazy<IConfiguration, IConfigurationMetadata>> Configurations
        {
            get
            {
                return _configurations;
            }
        }

        #endregion Properties

        #region Methods

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var configuration in Configurations)
            {
                configuration.Value.Initialize(new ConfigurationContext());
            }

            RaisePropertyChanged("Configurations");
        }

        private void ExecuteApply()
        {
            foreach (var configuration in _configurations)
            {
                if (configuration.IsValueCreated)
                {
                    configuration.Value.Commit();
                }
            }

            CloseSettingsView();
        }

        private void ExecuteCancel()
        {
            foreach (var configuration in _configurations)
            {
                if  (configuration.IsValueCreated)
                {
                    configuration.Value.Cancel();
                }
            }

            CloseSettingsView();
        }

        private void CloseSettingsView()
        {
            var region = _regionManager.Regions[RegionNames.MainRegion];
            var settingsView = region.GetView("Settings");

            if (settingsView != null)
            {
                region.Remove(settingsView);
            }
        }

        #endregion Methods

        private class ConfigurationContext : IConfigurationContext
        {
            
        }
    }
}