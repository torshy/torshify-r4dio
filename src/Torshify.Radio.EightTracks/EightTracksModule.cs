using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace Torshify.Radio.EightTracks
{
    [ModuleExport(typeof(EightTracksModule), DependsOnModuleNames = new[] { "Core" })]
    public class EightTracksModule : IModule
    {
        public void Initialize()
        {
        }
    }
}