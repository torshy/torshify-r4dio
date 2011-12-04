using System;
using System.ComponentModel.Composition;
using System.Windows;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Browse
{
    //[RadioStationMetadata(Name = "Browse", Icon = "MS_0000s_0036_search.png")]
    public class BrowseStation : IRadioStation
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public BrowseStation(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion("BrowseMainRegion", typeof(BrowseViewStartPage));
            _regionManager.RegisterViewWithRegion("BrowseMainRegion", typeof(SearchResultsView));
            _regionManager.RegisterViewWithRegion("BrowseMainRegion", typeof(ArtistBrowseView));
        }

        #endregion Constructors

        #region Methods

        public void Initialize(IRadio radio)
        {
        }

        public void OnTunedAway()
        {
        }

        public void OnTunedIn(IRadioStationContext context)
        {
            Lazy<UIElement> factory = new Lazy<UIElement>(CreateView);

            ViewData viewData = new ViewData();
            viewData.Header = "Browse";
            viewData.View = factory;

            context.SetView(viewData);
        }

        private UIElement CreateView()
        {
            return ServiceLocator.Current.TryResolve<BrowseView>();
        }

        #endregion Methods
    }
}