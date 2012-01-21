using System.ComponentModel.Composition;

namespace Torshify.Radio.Framework
{
    [InheritedExport]
    public interface IStartable
    {
        void Start();
    }
}