using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EightTracks
{
    public class EightTrackSearchTypeModel : NotificationObject
    {
        public string Text { get; set; }
        public EightTracksSearchType SearchType { get; set; }
    }
}