using System;
using System.ComponentModel.Composition;

namespace Torshify.Radio.Framework
{
    public interface ITrackSourceMetadata
    {
        #region Properties

        string Name
        {
            get;
        }

        string IconUri
        {
            get;
        }

        #endregion Properties
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TrackSourceMetadataAttribute : ExportAttribute
    {
        #region Constructors

        public TrackSourceMetadataAttribute()
            : base(typeof(ITrackSource))
        {
        }

        #endregion Constructors

        #region Properties

        public string Name
        {
            get; set;
        }

        public string IconUri
        {
            get; set;
        }

        #endregion Properties
    }
}