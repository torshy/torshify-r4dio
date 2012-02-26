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

    public interface ISettingsSection : IHeaderInfoProvider<HeaderInfo>
    {
        object UI
        {
            get;
        }

        void Load();

        void Save();
    }
}