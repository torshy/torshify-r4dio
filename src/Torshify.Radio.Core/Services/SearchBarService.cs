using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Services
{
    [Export(typeof(ISearchBarService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SearchBarService : NotificationObject, ISearchBarService
    {
        #region Fields

        private ObservableCollection<SearchBar> _bars;
        private SearchBar _current;
        private SearchBar _previous;

        #endregion Fields

        #region Constructors

        public SearchBarService()
        {
            _bars = new ObservableCollection<SearchBar>();
        }

        #endregion Constructors

        #region Events

        public event EventHandler CurrentChanged;

        #endregion Events

        #region Properties

        public SearchBar Current
        {
            get { return _current; }
            set
            {
                if (_current != value)
                {
                    _previous = _current;
                    _current = value;
                    RaisePropertyChanged("Current");
                    OnCurrentChanged();
                }
            }
        }

        public IEnumerable<SearchBar> SearchBars
        {
            get { return _bars; }
        }

        #endregion Properties

        #region Methods

        public void Add<T>(SearchBarData searchBarData, params Tuple<string, string>[] parameters)
        {
            UriQuery query = new UriQuery();

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    query.Add(parameter.Item1, parameter.Item2);
                }
            }

            Uri navigationUri = new Uri(typeof(T).FullName, UriKind.RelativeOrAbsolute);
            _bars.Add(new SearchBar(navigationUri, query, searchBarData));

            if (_bars.Count == 1 && Current == null)
            {
                Current = _bars.FirstOrDefault();
            }
        }

        public void Remove(SearchBar searchBar)
        {
            if (searchBar == Current)
            {
                Current = _previous;
            }

            _bars.Remove(searchBar);
        }

        public void Remove(Predicate<SearchBar> predicate)
        {
            List<SearchBar> results = SearchBars.Where(searchBar => predicate(searchBar)).ToList();
            results.ForEach(Remove);
        }

        public void SetActive(SearchBar searchBar)
        {
            Current = searchBar;
        }

        public void SetActive(Predicate<SearchBar> predicate)
        {
            foreach (var searchBar in SearchBars)
            {
                if (predicate(searchBar))
                {
                    Current = searchBar;
                    break;
                }
            }
        }

        protected void OnCurrentChanged()
        {
            var handler = CurrentChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}