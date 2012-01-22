using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.Framework
{
    public interface IRadioStation
    {
        void OnTuneIn(NavigationContext context);
        void OnTuneAway(NavigationContext context);
    }
}