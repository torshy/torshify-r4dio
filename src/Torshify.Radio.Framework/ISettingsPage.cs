using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Torshify.Radio.Framework
{
    [InheritedExport]
    public interface ISettingsPage : IHeaderInfoProvider<HeaderInfo>
    {
        IEnumerable<ISettingsSection> Sections
        {
            get;
        }
    }
}