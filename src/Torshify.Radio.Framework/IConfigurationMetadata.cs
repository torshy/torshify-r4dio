using System;
using System.ComponentModel.Composition;

namespace Torshify.Radio.Framework
{
    public interface IConfigurationMetadata
    {
        #region Properties

        string Name
        {
            get;
        }

        string Icon
        {
            get;
        }

        #endregion Properties
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigurationMetadataAttribute : ExportAttribute
    {
        #region Constructors

        public ConfigurationMetadataAttribute()
            : base(typeof(IConfiguration))
        {
        }

        #endregion Constructors

        #region Properties

        public string Name
        {
            get;
            set;
        }

        public string Icon
        {
            get;
            set;
        }

        #endregion Properties
    }
}