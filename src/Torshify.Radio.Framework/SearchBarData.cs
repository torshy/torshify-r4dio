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

        #region Properties

        public string Category
        {
            get
            {
                try
                {
                    string uiString;
                    LocTextExtension locExtension = new LocTextExtension(_category);
                    locExtension.Assembly = ResourceAssembly;
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

        public string ResourceAssembly
        {
            get; 
            set;
        }

        #endregion Properties
    }
}