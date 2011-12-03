using System.Threading;
using System.Threading.Tasks;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.TopHot
{
    [RadioStationMetadata(Name = "Hot artists", Icon = "MB_0014_msg3.png")]
    public class TopHotttTracksRadioStation : IRadioStation
    {
        #region Methods

        public void Initialize(IRadio radio)
        {
        }

        public void OnTunedAway()
        {
        }

        public void OnTunedIn(IRadioStationContext context)
        {
            var trackEnumerator = new TopHotttEnumerator(context);
            context.SetView(new ViewData { Header = "Top hot", IsEnabled = false });
            context
                .SetTrackProvider(trackEnumerator.DoIt)
                .ContinueWith(
                    t => context.GoToTracks(),
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion Methods
    }
}