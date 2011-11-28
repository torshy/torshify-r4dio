using System;
using System.Windows;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Mood
{
    [RadioStationMetadata(Name = "Moods", Icon = "MB_0003_Favs1.png")]
    public class MoodRadioStation : IRadioStation
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
                                     Header = "Moods", 
                                     View = new Lazy<UIElement>(() => new MoodRadioStationView
                                                                {
                                                                    DataContext = new MoodRadioStationViewModel(_radio, context)
                                                                })
                                 });
        }

        #endregion Methods
    }
}