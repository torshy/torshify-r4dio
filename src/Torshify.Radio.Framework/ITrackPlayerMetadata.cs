using System;
using System.ComponentModel.Composition;

namespace Torshify.Radio.Framework
{
    public interface ITrackPlayerMetadata
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
    public class TrackPlayerMetadataAttribute : ExportAttribute
    {
        #region Constructors

        public TrackPlayerMetadataAttribute()
            : base(typeof(ITrackPlayer))
        {
        }

        #endregion Constructors

        #region Properties

        public string Name
        {
            get;
            set;
        }

        public string IconUri
        {
            get;
            set;
        }

        #endregion Properties
    }
}