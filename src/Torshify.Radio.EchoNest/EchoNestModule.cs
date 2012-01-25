using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace Torshify.Radio.EchoNest
{
    [ModuleExport(typeof(EchoNestModule), DependsOnModuleNames = new[] { "Core" })]
    public class EchoNestModule : IModule
    {
        public void Initialize()
        {

        }
    }
}