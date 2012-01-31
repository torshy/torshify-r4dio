using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

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

        #endregion Properties

        #region Methods

        public void OnTuneAway(NavigationContext context)
        {
        }

        public void OnTuneIn(NavigationContext context)
        {
            IEnumerable<Track> tracks = Radio.GetTracksByName("NOFX").OrderBy(t => t.TotalDuration).Take(2);
            IEnumerable<TrackContainer> albums = Radio.GetAlbumsByArtist("NOFX").OrderBy(a => a.Year).ThenBy(a => a.Name);

            Radio.PlayTrackStream(new TrackSource(tracks));
        }

        #endregion Methods
    }
}