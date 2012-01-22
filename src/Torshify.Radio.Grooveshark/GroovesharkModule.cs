using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace Torshify.Radio.Grooveshark
{
    [ModuleExport(typeof(GroovesharkModule), DependsOnModuleNames = new[] { "Core" })]
    public class GroovesharkModule : IModule
    {
        public void Initialize()
        {
        }
    }
}