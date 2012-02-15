using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Windows.Controls;

using Raven.Client;

using Torshify.Radio.Core.Models;
using Torshify.Radio.Core.Views.Stations;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.Core.Views.FirstTime
{
    [Export(typeof(FirstTimeUseViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public class FirstTimeUseViewModel : NotificationObject, INavigationAware
    {
        #region Constructors

        public FirstTimeUseViewModel()
        {
            FinishCommand = new StaticCommand(ExecuteFinish);
        }

        #endregion Constructors

        #region Properties

        [Import]
        public IDocumentStore DocumentStore
        {
            get;
            set;
        }

        public ICommand FinishCommand
        {
            get;
            private set;
        }

        [Import]
        public ILoggerFacade Logger
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
        public IEnumerable<WizardPage> WizardPages
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        private void ExecuteFinish()
        {
            try
            {
                using (var session = DocumentStore.OpenSession())
                {
                    var settings = session.Query<ApplicationSettings>().FirstOrDefault();

                    if (settings == null)
                    {
                        settings = new ApplicationSettings();
                    }

                    settings.FirstTimeWizardRun = true;
                    session.Store(settings);
                    session.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, Category.Exception, Priority.Medium);
            }

            RegionManager.RequestNavigate(AppRegions.MainRegion, typeof(MainView).FullName);
            RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof(StationsView).FullName);
        }

        #endregion Methods
    }
}