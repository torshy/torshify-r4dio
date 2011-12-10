using System;
using System.Windows;
using Microsoft.Practices.Prism;
using Microsoft.Practices.ServiceLocation;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Browse
{
    [RadioStationMetadata(Name = "Search", Icon = "MS_0000s_0036_search.png")]
    public class BrowseStation : IRadioStation
    {
        #region Methods

        public void Initialize(IRadio radio)
        {
        }

        public void OnTunedAway()
        {
        }

        public void OnTunedIn(IRadioStationContext context)
        {
            var factory = new Lazy<UIElement>(() =>
            {
                var view = ServiceLocator.Current.TryResolve<BrowseView>();
                return view;
            });

            var viewData = new ViewData();
            viewData.Header = "Browse";
            viewData.View = factory;

            context.SetView(viewData);
        }

        #endregion Methods
    }
}