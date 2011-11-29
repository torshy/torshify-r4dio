using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EchoNest;
using EchoNest.Artist;
using Torshify.Radio.Framework;
using System.Linq;

namespace Torshify.Radio.EchoNest.TopHot
{
    [RadioStationMetadata(Name = "Hot artists", Icon = "MB_0014_msg3.png")]
    public class TopHotttTracksRadioStation : IRadioStation
    {
        #region Fields

        private int _batchSize;
        private int _currentOffset;
        private IRadio _radio;

        #endregion Fields

        #region Methods

        public void Initialize(IRadio radio)
        {
            _radio = radio;
        }

        public void OnTunedAway()
        {
        }

        public void OnTunedIn(IRadioStationContext context)
        {
            _batchSize = 5;
            _currentOffset = 0;

            context.SetView(new ViewData { Header = "Top hot", IsEnabled = false });
            context
                .SetTrackProvider(GetTaskFactory)
                .ContinueWith(t => context.GoToTracks(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private IEnumerable<IRadioTrack> ConvertToTracks(TopHotttResponse response)
        {
            var tracks = new List<IRadioTrack>();

            if (response.Status.Code == ResponseCode.Success)
            {
                _currentOffset += response.Artists.Count;

                foreach (var artist in response.Artists.OrderBy(t => Guid.NewGuid()))
                {
                    var artistTracks = _radio.GetTracksByArtist(artist.Name, 0, 1);
                    tracks.AddRange(artistTracks);
                }
            }

            return tracks;
        }

        private IEnumerable<IRadioTrack> GetTaskFactory()
        {
            return Task.Factory
                .StartNew(() => GetTopHotttArtists())
                .ContinueWith(t => ConvertToTracks(t.Result)).Result;
        }

        private TopHotttResponse GetTopHotttArtists()
        {
            try
            {
                using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
                {
                    return session.Query<TopHottt>().Execute(_batchSize, _currentOffset);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new TopHotttResponse
                       {
                           Status = new ResponseStatus
                                        {
                                            Code = ResponseCode.UnknownError
                                        }
                       };
        }

        #endregion Methods
    }
}