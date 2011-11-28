using System;
using System.Windows;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.GeneralSearch
{
    [RadioStationMetadata(Name = "Search", Icon = "MS_0000s_0036_search.png")]
    public class SearchRadioStation : IRadioStation
    {
        private IRadio _radio;

        public void Initialize(IRadio radio)
        {
            _radio = radio;
        }

        public void OnTunedAway()
        {
            
        }

        public void OnTunedIn(IRadioStationContext context)
        {
            var viewFactory = new Lazy<UIElement>(()=>
                                                {
                                                    var viewModel = new SearchRadioStationViewModel(_radio, context);
                                                    var view = new SearchRadioStationView(viewModel);
                                                    return view;
                                                });

            var viewData = new ViewData();
            viewData.View = viewFactory;
            viewData.Header = "Search";

            context.SetView(viewData);
        }
    }
}