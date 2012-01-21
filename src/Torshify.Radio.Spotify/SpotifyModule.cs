using System;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using Raven.Client;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Spotify
{
    [ModuleExport(typeof(SpotifyModule), DependsOnModuleNames = new[] { "Database" })]
    public class SpotifyModule : IModule
    {
        #region Properties

        [Import]
        public IDocumentStore DocumentStore
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            TrackLink link = new TrackLink("spotify");
            link["url"] = "spotify:track:5jJBcAaSqUFqcD4A3CSSvC";

            string uri = link.Uri;

            Console.WriteLine(uri);

            TrackLink result = TrackLink.FromUri(uri);

            Console.WriteLine(result.Uri);
            Console.WriteLine(result["url"]);
            Console.WriteLine();
        }

        #endregion Methods
    }
}