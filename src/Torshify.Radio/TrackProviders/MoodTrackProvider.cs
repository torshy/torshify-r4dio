using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EchoNest;
using EchoNest.Artist;

using Torshify.Origo.Contracts.V1;
using Torshify.Origo.Contracts.V1.Query;
using Torshify.Radio.;

namespace Torshify.Radio.TrackProviders
{
    public class MoodTrackProvider : ITrackProvider
    {
        #region Constructors

        public MoodTrackProvider()
        {
            BatchSize = 15;
            CurrentOffset = 0;
            NumberOfTracksPerArtist = 5;
            Moods = new List<string>();
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

        public List<string> Moods
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

        public static IEnumerable<string> GetAvailableMoods()
        {
            EchoNestSession session = new EchoNestSession("RJOXXESVUVZ07WY1T");
            var response = session
                .Query<ListTerms>()
                .Execute(ListTermsType.Mood);

            if (response.Status.Code == ResponseCode.Success)
            {
                return response.Terms.Select(t => t.Name);
            }

            return new string[] {};
        }

        public Task<IEnumerable<Track>> GetNextTrackBatch()
        {
            EchoNestSession session = new EchoNestSession("RJOXXESVUVZ07WY1T");
            var searchArgument = new SearchArgument
                                     {
                                         Results = BatchSize, Start = CurrentOffset
                                     };

            foreach (var mood in Moods)
            {
                searchArgument.Moods.Add(mood);
            }

            return session
                .Query<Search>()
                .ExecuteAsync(searchArgument)
                .ContinueWith(t => GetBatch(session, t.Result));
        }

        private IEnumerable<Track> GetBatch(EchoNestSession session, SearchResponse response)
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