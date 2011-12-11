using System;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Events;

namespace Torshify.Radio.EchoNest.Similar
{
    [RadioStationMetadata(Name = "Similar artists", Icon = "MS_0000s_0031_net3.png")]
    public class SimilarArtistsRadioStation : IRadioStation
    {
        #region Fields

        private IRadio _radio;

        #endregion Fields

        #region Methods

        public void Initialize(IRadio radio)
        {
            _radio = radio;
            _radio.GetService<IEventAggregator>().GetEvent<ArtistCommandsEvent>().Subscribe(OnArtistCommandsEvent);
        }

        public void OnTunedAway()
        {
        }

        public void OnTunedIn(IRadioStationContext context)
        {
            var viewFactory = new Lazy<UIElement>(()=>
                                                {
                                                    var viewModel = new SimilarArtistsRadioStationViewModel(_radio);
                                                    var view = new SimilarArtistsRadioStationView(viewModel);
                                                    return view;
                                                });

            ViewData viewData = new ViewData();
            viewData.Header = "Similar artists";
            viewData.View = viewFactory;

            context.SetView(viewData);
        }

        private void OnArtistCommandsEvent(ArtistCommandsPayload payload)
        {
            payload
                .CommandBar
                .AddCommand(
                    "Similar artists",
                    new DelegateCommand<string>(ExecuteFindSimilarArtists), payload.ArtistName);
        }

        private void ExecuteFindSimilarArtists(string artistName)
        {
            var viewFactory = new Lazy<UIElement>(() =>
                                                      {
                                                          var viewModel = new SimilarArtistsRadioStationViewModel(_radio);
                                                          var view = new SimilarArtistsRadioStationView(viewModel);
                                                          viewModel.SearchCommand.Execute(artistName);
                                                          return view;
                                                      });

            ViewData viewData = new ViewData();
            viewData.Header = "Similar artists";
            viewData.View = viewFactory;

            _radio.CurrentContext.SetView(viewData);
        }

        #endregion Methods
    }
}