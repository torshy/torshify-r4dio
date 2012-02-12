using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Prism.ViewModel;

using WPFLocalizeExtension.Extensions;

namespace Torshify.Radio.Framework
{
    public class SearchBarData : NotificationObject
    {
        #region Fields

        private string _category;
        private string _watermarkText;

        #endregion Fields

        #region Constructors

        public SearchBarData()
        {
            AutoCompleteProvider = phrase => new string[0];
            ResourceAssembly = Assembly.GetCallingAssembly().GetName().Name;
            ResourceName = "Strings";
        }

        #endregion Constructors

        #region Properties

        public Func<string, IEnumerable<string>> AutoCompleteProvider
        {
            get;
            set;
        }

        public string Category
        {
            get
            {
                try
                {
                    string uiString;
                    LocTextExtension locExtension = new LocTextExtension(_category);
                    locExtension.Assembly = ResourceAssembly;
                    locExtension.Dict = ResourceName;
                    locExtension.ResolveLocalizedValue(out uiString);
                    return uiString;
                }
                catch
                {

                }

                return _category;
            }
            set
            {
                _category = value;
            }
        }

        public string ResourceAssembly
        {
            get;
            set;
        }

        public string ResourceName
        {
            get; 
            set;
        }

        public string WatermarkText
        {
            get
            {
                try
                {
                    string uiString;
                    LocTextExtension locExtension = new LocTextExtension(_watermarkText);
                    locExtension.Assembly = ResourceAssembly;
                    locExtension.ResolveLocalizedValue(out uiString);
                    return uiString;
                }
                catch
                {

                }

                return _watermarkText;
            }
            set
            {
                _watermarkText = value;
            }
        }

        #endregion Properties
    }
}