using System;

namespace Torshify.Radio.Framework
{
    public interface IBackdropService
    {
        #region Properties

        string CacheLocation
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        void GetBackdrop(string artistName, Action<string> foundBackdrop, Action didNotFindBackdrop = null);

        bool TryGetFromCache(string artistName, out string fileName);

        bool TryGetFromCache(string artistName, out string[] fileNames);

        #endregion Methods
    }
}