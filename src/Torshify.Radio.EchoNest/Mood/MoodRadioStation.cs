using System;
using System.IO;
using System.Windows;

using Microsoft.Isam.Esent.Collections.Generic;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Mood
{
    [RadioStationMetadata(Name = "Moods", Icon = "MB_0003_Favs1.png")]
    public class MoodRadioStation : IRadioStation
    {
        #region Fields

        public static PersistentDictionary<string, int> MoodCloudData;

        private IRadioStationContext _context;
        private IRadio _radio;

        #endregion Fields

        #region Constructors

        static MoodRadioStation()
        {
            MoodCloudData = new PersistentDictionary<string, int>(Path.Combine(AppConstants.AppDataFolder, "Data", "MoodCloud"));
        }

        #endregion Constructors

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