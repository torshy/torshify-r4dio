using System;
using System.Windows;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Events;

namespace Torshify.Radio.EchoNest.Browse
{
    [RadioStationMetadata(Name = "Search", Icon = "MS_0000s_0036_search.png")]
    public class BrowseStation : IRadioStation
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
            var factory = new Lazy<UIElement>(() =>
            {
                var view = ServiceLocator.Current.TryResolve<BrowseView>();
                return view;
            });

            var viewData = new ViewData();
            viewData.Header = "Search";
            viewData.View = factory;

            context.SetView(viewData);
        }

        private void ExecuteFindAlbums(string artistName)
        {
            var factory = new Lazy<UIElement>(() =>
            {
                var view = ServiceLocator.Current.TryResolve<BrowseView>();
                UriQuery uri = new UriQuery();
                uri.Add("name", artistName);
                view.Model.RegionManager.RequestNavigate("BrowseMainRegion", new Uri(typeof(ArtistBrowseView).FullName + uri, UriKind.Relative));
                return view;
            });

            var viewData = new ViewData();
            viewData.Header = "Search";
            viewData.View = factory;

            _radio.CurrentContext.SetView(viewData);
        }

        private void OnArtistCommandsEvent(ArtistCommandsPayload payload)
        {
            payload
                .CommandBar
                .AddCommand(
                    "Browse",
                    new DelegateCommand<string>(ExecuteFindAlbums), payload.ArtistName);
        }

        #endregion Methods
    }
}