using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using EchoNest;
using EchoNest.Artist;

using Torshify.Origo.Contracts.V1;
using Torshify.Origo.Contracts.V1.Query;
using Torshify.Radio.QueryService;

namespace Torshify.Radio.TrackProviders
{
    public class TopHotttTrackProvider : ITrackProvider
    {
        #region Constructors

        public TopHotttTrackProvider()
        {
            BatchSize = 15;
            CurrentOffset = 0;
            NumberOfTracksPerArtist = 5;
        }

        #endregion Constructors

        #region Properties

        public int BatchSize
        {
            get;
            set;
        }

        public int CurrentOffset
        {
            get;
            set;
        }

        public int NumberOfTracksPerArtist
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public Task<IEnumerable<Track>> GetNextTrackBatch()
        {
            EchoNestSession session = new EchoNestSession("RJOXXESVUVZ07WY1T");
            return session
                .Query<TopHottt>()
                .ExecuteAsync(BatchSize, CurrentOffset)
                .ContinueWith(t => GetBatch(session, t.Result));
        }

        private IEnumerable<Track> GetBatch(EchoNestSession session, TopHotttResponse response)
        {
            var tracks = new List<Track>();
            var query = new QueryServiceClient();

            try
            {
                if (response.Status.Code == ResponseCode.Success)
                {
                    CurrentOffset += response.Artists.Count;

                    foreach (var artist in response.Artists)
                    {
                        QueryResult result = query.Query(artist.Name, 0, NumberOfTracksPerArtist, 0, 0, 0, 0);
                        tracks.AddRange(result.Tracks);
                    }
                }

                query.Close();
            }
            catch (Exception)
            {
                query.Abort();
            }

            try
            {
                using (session)
                {
                }
            }
            catch
            {
            }

            return tracks;
        }

        #endregion Methods
    }
}