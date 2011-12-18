using System;
using System.IO;
using System.Windows;

using Microsoft.Isam.Esent.Collections.Generic;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Style
{
    [RadioStationMetadata(Name = "Styles", Icon = "MB_0029_programs.png")]
    public class StyleRadioStation : IRadioStation
    {
        #region Fields

        public static PersistentDictionary<string, int> StyleCloudData;

        private IRadioStationContext _context;
        private IRadio _radio;

        #endregion Fields

        #region Constructors

        static StyleRadioStation()
        {
            StyleCloudData = new PersistentDictionary<string, int>(Path.Combine(AppConstants.AppDataFolder, "Data", "StyleCloud"));
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