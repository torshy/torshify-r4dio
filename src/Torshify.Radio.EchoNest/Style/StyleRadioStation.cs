using System;
using System.Windows;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Style
{
    [RadioStationMetadata(Name = "Styles", Icon = "MB_0029_programs.png")]
    public class StyleRadioStation : IRadioStation
    {
        #region Fields

        private IRadio _radio;
        private IRadioStationContext _context;

        #endregion Fields

        #region Methods

        public void Initialize(IRadio radio)
        {
            _radio = radio;
        }

        public void OnTunedAway()
        {
            _context = null;
        }

        public void OnTunedIn(IRadioStationContext context)
        {
            _context = context;
            _context.SetView(new ViewData
                                 {
                                     Header = "Styles", 
                                     View = new Lazy<UIElement>(() => new StyleRadioStationView
                                                                {
                                                                    DataContext = new StyleRadioStationViewModel(_radio, context)
                                                                })
                                 });
        }

        #endregion Methods
    }
}