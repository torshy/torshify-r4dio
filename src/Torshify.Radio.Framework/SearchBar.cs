using System;

using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Framework
{
    public class SearchBar : NotificationObject
    {
        #region Constructors

        public SearchBar(Uri navigationUri, SearchBarData data)
        {
            NavigationUri = navigationUri;
            Data = data;
        }

        #endregion Constructors

        #region Properties

        public SearchBarData Data
        {
            get; private set;
        }

        public Uri NavigationUri
        {
            get; private set;
        }

        #endregion Properties
    }
}