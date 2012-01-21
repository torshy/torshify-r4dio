using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace Torshify.Radio.Database
{
    [ModuleExport("Database", typeof(DatabaseModule))]
    public class DatabaseModule : IModule
    {
        public void Initialize()
        {
        }
    }
}
