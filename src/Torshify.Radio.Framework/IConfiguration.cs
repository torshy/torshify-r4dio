using System.Windows;

namespace Torshify.Radio.Framework
{
    public interface IConfiguration
    {
        FrameworkElement UI { get; }

        void Commit();
        void Cancel();
    }
}