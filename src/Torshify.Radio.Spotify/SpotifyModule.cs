using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Torshify.Radio.Framework;


namespace Torshify.Radio.Spotify
{
    [ModuleExport(typeof(SpotifyModule), DependsOnModuleNames = new[] { "Core" })]
    public class SpotifyModule : IModule
    {
        #region Methods

        public void Initialize()
        {

        }

        #endregion Methods
    }

    [RadioStationMetadata(Name = "Spotify")]
    public class SpotifyStation : IRadioStation
    {
        [Import]
        public IRadio Radio
        {
            get; 
            set;
        }

        public void OnTuneIn()
        {
        }

        public void OnTuneAway()
        {
        }
    }
}