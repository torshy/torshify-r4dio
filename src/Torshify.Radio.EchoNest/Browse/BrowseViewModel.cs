using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism;

namespace Torshify.Radio.EchoNest.Browse
{
    [Export(typeof(BrowseViewModel))]
    public class BrowseViewModel : AutoCompleteViewModel
    {
        #region Constructors

        public BrowseViewModel()
        {
            SearchCommand = new DelegateCommand<string>(ExecuteSearch);
        }

        #endregion Constructors

        #region Properties

        public ICommand SearchCommand
        {
            get;
            private set;
        }

        public IRegionManager RegionManager
        {
            get; 
            set;
        }

        #endregion Properties

        #region Methods

        private void ExecuteSearch(string query)
        {
            UriQuery uri = new UriQuery();
            uri.Add("Query", query);

            RegionManager.RequestNavigate("BrowseMainRegion", new Uri("SearchResultsView" + uri, UriKind.Relative));
        }

        #endregion Methods
    }
}