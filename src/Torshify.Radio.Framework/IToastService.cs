using System.Windows;

namespace Torshify.Radio.Framework
{
    public interface IToastService
    {
        void Show(string message, double displayTimeMs = 1000);

        void Show(UIElement element, double displayTimeMs = 1000);
    }
}