using System;
using System.ComponentModel.Composition;

namespace Torshify.Radio.Framework
{
    public interface IRadioStationMetadata
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
    public class RadioStationMetadataAttribute : ExportAttribute
    {
        #region Constructors

        public RadioStationMetadataAttribute()
            : base(typeof(IRadioStation))
        {
        }

        #endregion Constructors

        #region Properties

        public string Name
        {
            get; set;
        }

        public string Icon
        {
            get; 
            set;
        }

        #endregion Properties
    }
}