using System;
using System.ComponentModel.Composition;
using System.Threading;

using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Framework
{
    [Export(typeof(ILoadingIndicatorService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LoadingIndicatorService : NotificationObject, ILoadingIndicatorService
    {
        #region Fields

        private int _counter;
        private bool _isLoading;

        #endregion Fields

        #region Properties

        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    RaisePropertyChanged("IsLoading");
                }
            }
        }

        #endregion Properties

        #region Methods

        public IDisposable EnterLoadingBlock()
        {
            return new IncrementDecrementDisposable(this);
        }

        public void Pop()
        {
            if (Interlocked.Decrement(ref _counter) == 0)
            {
                IsLoading = false;
            }
        }

        public void Push()
        {
            if (Interlocked.Increment(ref _counter) != 0)
            {
                IsLoading = true;
            }
        }

        #endregion Methods

        #region Nested Types

        private class IncrementDecrementDisposable : IDisposable
        {
            #region Fields

            private readonly LoadingIndicatorService _parent;

            #endregion Fields

            #region Constructors

            public IncrementDecrementDisposable(LoadingIndicatorService parent)
            {
                _parent = parent;
                _parent.Push();
            }

            #endregion Constructors

            #region Methods

            public void Dispose()
            {
                _parent.Pop();
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}