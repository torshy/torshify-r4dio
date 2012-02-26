﻿using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using System.Linq;

namespace Torshify.Radio.Core.Views.Settings
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public class SettingsViewModel : NotificationObject, INavigationAware
    {
        private ISettingsPage _currentPage;

        #region Properties

        [ImportMany]
        public IEnumerable<ISettingsPage> SettingPages
        {
            get;
            set;
        }

        public ISettingsPage CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    RaisePropertyChanged("CurrentPage");
                }
            }
        }

        #endregion Properties

        #region Methods

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            SettingPages.ForEach(page => page.Sections.ForEach(section => section.Load()));
            CurrentPage = SettingPages.FirstOrDefault();
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            SettingPages.ForEach(page => page.Sections.ForEach(section => section.Save()));
        }

        #endregion Methods
    }
}