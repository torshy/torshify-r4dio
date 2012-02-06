namespace Torshify.Radio.Framework
{
    public interface IToastService
    {
        void Show(string message, int displayTimeMs = 2500);
        void Show(ToastData data);
    }
}