using System;
using System.Windows;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks
{
    [RadioStationMetadata(Name = "8tracks", Icon = "MB_9999_8tracks.png")]
    public class EightTracksRadioStation : IRadioStation
    {
        #region Fields

        internal const string ApiKey = "63b5cb8daf03ec1df8f1c25fec5479b612739a29";

        #endregion Fields

        #region Methods

        public void Initialize(IRadio radio)
        {
        }

        public void OnTunedAway()
        {
            EightTracksTrackManager.Instance.Deinitialize();
        }

        public void OnTunedIn(IRadioStationContext context)
        {
            EightTracksTrackManager.Instance.Initialize(context);

            var viewFactory = new Lazy<UIElement>(() =>
                                                           {
                                                               var viewModel = new EightTracksRadioStationViewModel(context);
                                                               var view = new EightTracksRadioStationView(viewModel);
                                                               return view;
                                                           });

            context.SetView(new ViewData
                                {
                                    Header = "8tracks",
                                    View = viewFactory
                                });
        }

        #endregion Methods
    }
}