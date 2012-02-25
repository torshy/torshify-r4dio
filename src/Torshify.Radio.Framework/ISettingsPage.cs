using System.ComponentModel.Composition;

namespace Torshify.Radio.Framework
{
    [InheritedExport]
    public interface ISettingsPage
    {
        void Load();
        void Save();
    }
}