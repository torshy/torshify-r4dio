using System;
using System.ComponentModel.Composition;

namespace Torshify.Radio.Framework
{
    public interface IRadioTrackPlayerMetadata
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
    public class RadioTrackPlayerMetadataAttribute : ExportAttribute
    {
        #region Constructors

        public RadioTrackPlayerMetadataAttribute()
            : base(typeof(IRadioTrackPlayer))
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