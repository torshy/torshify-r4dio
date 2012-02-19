using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

using Raven.Client;

using Torshify.Radio.Core.Models;
using Torshify.Radio.Core.Views;
using Torshify.Radio.Core.Views.Controls;
using Torshify.Radio.Core.Views.FirstTime;
using Torshify.Radio.Core.Views.Settings;
using Torshify.Radio.Core.Views.Stations;
using Torshify.Radio.Framework;

using WPFLocalizeExtension.Engine;

namespace Torshify.Radio.Core
{
    [ModuleExport("Core", typeof(CoreModule), DependsOnModuleNames = new[] { "Database" })]
    public class CoreModule : IModule
    {
        #region Properties

        [Import]
        public IDocumentStore DocumentStore
        {
            get;
            set;
        }

        [Import]
        public IRegionManager RegionManager
        {
            get;
            set;
        }

        [ImportMany]
        public IEnumerable<IStartable> Startables
        {
            get;
            set;
        }

        [Import("CorePlayer")]
        public ITrackPlayer Player
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            bool displayWizard;

            using (var session = DocumentStore.OpenSession())
            {
                var settings = session.Query<ApplicationSettings>().FirstOrDefault();

                if (settings == null)
                {
                    settings = new ApplicationSettings();
                    session.Store(settings);
                    session.SaveChanges();
                }

                displayWizard = !settings.FirstTimeWizardRun;

                if (!string.IsNullOrEmpty(settings.Culture))
                {
                    LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(settings.Culture);
                }
                else
                {
                    LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("en");
                }

                if (settings.AccentColor.HasValue)
                {
                    Application.Current.Resources[AppTheme.AccentColorKey] = settings.AccentColor.GetValueOrDefault();
                    Application.Current.Resources[AppTheme.AccentBrushKey] = new SolidColorBrush(settings.AccentColor.GetValueOrDefault());
                }
            }

            RegionManager.RegisterViewWithRegion(AppRegions.MainRegion, typeof(MainView));
            RegionManager.RegisterViewWithRegion(AppRegions.BottomRegion, typeof(ControlsView));
            RegionManager.RegisterViewWithRegion(AppRegions.ViewRegion, typeof(SettingsView));
            RegionManager.RegisterViewWithRegion(AppRegions.ViewRegion, typeof(StationsView));

            if (displayWizard)
            {
                RegionManager.RequestNavigate(AppRegions.MainRegion, typeof(FirstTimeUseView).FullName);
            }
            else
            {
                RegionManager.RequestNavigate(AppRegions.MainRegion, typeof(MainView).FullName);
                RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof(StationsView).FullName);
            }

            foreach (var startable in Startables)
            {
                startable.Start();
            }

            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
        }

        private void CurrentDomainOnProcessExit(object sender, EventArgs e)
        {
            if (Player != null)
            {
                Player.Dispose();
            }
        }

        #endregion Methods
    }
}