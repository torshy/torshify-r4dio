using System;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Framework
{
    public class SearchBar : NotificationObject
    {
        #region Fields

        public const string IsFromSearchBarParameter = "IsFromSearchBar";
        public const string ValueParameter = "Value";

        #endregion Fields

        #region Constructors

        public SearchBar(Uri navigationUri, UriQuery parameters, SearchBarData data)
        {
            NavigationUri = navigationUri;
            Data = data;
            Parameters = parameters;
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

        public UriQuery Parameters
        {
            get;  private set;
        }

        #endregion Properties
    }
}