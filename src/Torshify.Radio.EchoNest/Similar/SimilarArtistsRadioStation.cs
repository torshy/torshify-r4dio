using System;
using System.Windows;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Similar
{
    [RadioStationMetadata(Name = "Similar artists", Icon = "MS_0000s_0031_net3.png")]
    public class SimilarArtistsRadioStation : IRadioStation
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
                                                    var viewModel = new SimilarArtistsRadioStationViewModel(_radio, context);
                                                    var view = new SimilarArtistsRadioStationView(viewModel);
                                                    return view;
                                                });

            ViewData viewData = new ViewData();
            viewData.Header = "Similar artists";
            viewData.View = viewFactory;

            context.SetView(viewData);
        }
    }
}