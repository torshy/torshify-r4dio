using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Views.Settings.General
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralSettingsViewModel : NotificationObject, ISettingsPage
    {
        #region Fields

        [ImportMany("GeneralSettingsSection", typeof(ISettingsSection))]
        private IEnumerable<ISettingsSection> _sections = null;

        #endregion Fields

        #region Constructors

        public GeneralSettingsViewModel()
        {
            HeaderInfo = new HeaderInfo
            {
                Title = "General settings",
                IconUri = AppIcons.Settings.ToString()
            };
        }

        #endregion Constructors

        #region Properties

        public HeaderInfo HeaderInfo
        {
            get; private set;
        }

        public IEnumerable<ISettingsSection> Sections
        {
            get { return _sections; }
        }

        #endregion Properties
    }
}