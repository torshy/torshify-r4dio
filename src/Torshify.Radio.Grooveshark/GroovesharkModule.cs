using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using SciLorsGroovesharkAPI.Groove;

namespace Torshify.Radio.Grooveshark
{
    [ModuleExport(typeof(GroovesharkModule), DependsOnModuleNames = new[] { "Core" })]
    public class GroovesharkModule : IModule
    {
        #region Properties

           [Import]
        public ILoggerFacade Logger
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods
        public void Initialize()
        {
            
        }

        #endregion Methods
    }
}