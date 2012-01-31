using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Spotify.Views
{
    [Export(typeof(MainStationViewModel))]
    public class MainStationViewModel : NotificationObject, IRadioStation
    {
        #region Properties

        [Import]
        public IRadio Radio
        {
            get;
            set;
        }

        [Import]
        public ILoadingIndicatorService LoadingIndicatorService
        {
            get; 
            set;
        }

        #endregion Properties

        #region Methods

        public void OnTuneAway(NavigationContext context)
        {
        }

        public void OnTuneIn(NavigationContext context)
        {
            Task.Factory.StartNew(() =>
                                  {
                                      using (LoadingIndicatorService.EnterLoadingBlock())
                                      {
                                          Radio.Play(
                                              Radio.GetTracksByName("NOFX").OrderBy(t => t.TotalDuration).Take(2).
                                                  ToTrackStream());
                                      }
                                  });
        }

        #endregion Methods
    }
}