using System;
using System.Collections.Generic;

namespace Torshify.Radio.Database
{
    internal class ThreadSafeStore<TKey, TValue>
    {
        #region Fields

        // Fields
        private readonly Func<TKey, TValue> _creator;
        private readonly object _lock;

        private Dictionary<TKey, TValue> _store;

        #endregion Fields

        #region Constructors

        // Methods
        public ThreadSafeStore(Func<TKey, TValue> creator)
        {
            this._lock = new object();
            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }
            this._creator = creator;
        }

        #endregion Constructors

        #region Methods

        public TValue Get(TKey key)
        {
            TValue value;
            if (this._store == null)
            {
                return this.AddValue(key);
            }
            if (!this._store.TryGetValue(key, out value))
            {
                return this.AddValue(key);
            }
            return value;
        }

        private TValue AddValue(TKey key)
        {
            TValue value = this._creator(key);
            lock (this._lock)
            {
                if (this._store == null)
                {
                    this._store = new Dictionary<TKey, TValue>();
                    this._store[key] = value;
                }
                else
                {
                    TValue checkValue;
                    if (this._store.TryGetValue(key, out checkValue))
                    {
                        return checkValue;
                    }
                    Dictionary<TKey, TValue> newStore = new Dictionary<TKey, TValue>(this._store);
                    newStore[key] = value;
                    this._store = newStore;
                }
                return value;
            }
        }

        #endregion Methods
    }
}