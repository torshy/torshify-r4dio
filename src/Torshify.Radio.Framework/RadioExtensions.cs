namespace Torshify.Radio.Framework
{
    public static class RadioExtensions
    {
        public static T GetService<T>(this IRadio radio) where T : class
        {
            return radio.GetService(typeof (T)) as T;
        }
    }
}