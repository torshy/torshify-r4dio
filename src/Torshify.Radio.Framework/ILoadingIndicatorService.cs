using System;

namespace Torshify.Radio.Framework
{
    public interface ILoadingIndicatorService
    {
        #region Properties

        bool IsLoading
        {
            get;
        }

        #endregion Properties

        #region Methods

        IDisposable EnterLoadingBlock();

        void Pop();

        void Push();

        #endregion Methods
    }
}