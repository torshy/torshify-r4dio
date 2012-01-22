using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Spotify.Views
{
    [Export(typeof(MainStationViewModel))]
    public class MainStationViewModel : NotificationObject, IRadioStation
    {
        #region Methods

        public void OnTuneAway(NavigationContext context)
        {
        }

        public void OnTuneIn(NavigationContext context)
        {
        }

        #endregion Methods
    }
}