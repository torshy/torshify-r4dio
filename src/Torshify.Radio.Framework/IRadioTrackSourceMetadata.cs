using System;
using System.ComponentModel.Composition;

namespace Torshify.Radio.Framework
{
    public interface IRadioTrackSourceMetadata
    {
        #region Properties

        string Name
        {
            get;
        }

        #endregion Properties
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RadioTrackSourceMetadataAttribute : ExportAttribute
    {
        #region Constructors

        public RadioTrackSourceMetadataAttribute()
            : base(typeof(IRadioTrackSource))
        {
        }

        #endregion Constructors

        #region Properties

        public string Name
        {
            get; set;
        }

        #endregion Properties
    }
}