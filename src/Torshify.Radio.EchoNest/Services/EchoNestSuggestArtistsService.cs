using System.ComponentModel.Composition;
using System.Linq;
using EchoNest;
using EchoNest.Artist;

namespace Torshify.Radio.EchoNest.Services
{
    [Export(typeof(ISuggestArtistsService))]
    public class EchoNestSuggestArtistsService : ISuggestArtistsService
    {
        public string[] GetSimilarArtists(string query)
        {
            using (EchoNestSession session = new EchoNestSession(EchoNestModule.ApiKey))
            {
                var response = session.Query<SuggestArtist>().Execute(query);

                if (response.Status.Code == ResponseCode.Success)
                {
                    return response.Artists.Select(t => t.Name).ToArray();
                }
            }

            return new string[0];
        }
    }
}