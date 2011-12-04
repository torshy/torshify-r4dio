using System;
using System.ComponentModel.Composition;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Research
{
    [RadioStationMetadata(Name = "Research", Icon = "MB_0019_profiles.png")]
    public class ResearchStation : IRadioStation
    {
        private IRegionManager _regionManager;

        [ImportingConstructor]
        public ResearchStation(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion("ResearchRegion", typeof(ResearchViewChild1));
            _regionManager.RegisterViewWithRegion("ResearchRegion", typeof(ResearchViewChild2));
        }

        public void Initialize(IRadio radio)
        {

        }

        public void OnTunedAway()
        {

        }

        public void OnTunedIn(IRadioStationContext context)
        {
            var regionManager = new RegionManager();


            Lazy<UIElement> viewFactory = new Lazy<UIElement>(() => new ResearchView(regionManager));

            ViewData viewData = new ViewData();
            viewData.Header = "Research";
            viewData.View = viewFactory;

            context.SetView(viewData);
        }
    }
}