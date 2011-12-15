using System;
using System.Windows;
using Microsoft.Practices.Prism;
using Microsoft.Practices.ServiceLocation;
using Torshify.Radio.EchoNest.Browse;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.TrackDJ
{
    [RadioStationMetadata(Name = "Track DJ", Icon = "MB_0015_voicemaill.png")]
    public class TrackDJStation : IRadioStation
    {
        #region Fields

        private IRadio _radio;

        #endregion Fields

        #region Methods

        public void Initialize(IRadio radio)
        {
            _radio = radio;
        }

        public void OnTunedAway()
        {
        }

        public void OnTunedIn(IRadioStationContext context)
        {
            context.SetView(new ViewData
            {
                Header = "Track DJ",
                View = new Lazy<UIElement>(() =>
                                               {
                                                   var view = ServiceLocator.Current.TryResolve<TrackDJView>();
                                                   return view;
                                               })
            });
        }

        #endregion Methods
    }
}