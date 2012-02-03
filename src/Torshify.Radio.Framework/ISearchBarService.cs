using System;
using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface ISearchBarService
    {
        #region Events

        event EventHandler CurrentChanged;

        #endregion Events

        #region Properties

        SearchBar Current
        {
            get;
            set;
        }

        IEnumerable<SearchBar> SearchBars
        {
            get;
        }

        #endregion Properties

        #region Methods

        void Add<T>(SearchBarData searchBarData, params Tuple<string, string>[] parameters);

        void SetActive(Predicate<SearchBar> predicate);

        #endregion Methods
    }
}