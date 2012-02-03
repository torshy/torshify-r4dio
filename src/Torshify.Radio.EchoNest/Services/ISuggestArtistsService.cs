namespace Torshify.Radio.EchoNest.Services
{
    public interface ISuggestArtistsService
    {
        string[] GetSimilarArtists(string query);
    }
}