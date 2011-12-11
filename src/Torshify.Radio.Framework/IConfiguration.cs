using System.Windows;

namespace Torshify.Radio.Framework
{
    public interface IConfiguration
    {
        FrameworkElement UI { get; }

        void Initialize(IConfigurationContext context);
        void Commit();
        void Cancel();
    }
}