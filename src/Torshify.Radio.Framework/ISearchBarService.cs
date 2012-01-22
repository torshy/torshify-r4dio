using System;
using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface ISearchBarService
    {
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

        #endregion Methods
    }
}