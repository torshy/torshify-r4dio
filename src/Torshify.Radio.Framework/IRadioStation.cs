namespace Torshify.Radio.Framework
{
    public interface IRadioStation
    {
        #region Methods

        void Initialize(IRadio radio);

        void OnTunedAway();

        void OnTunedIn(IRadioStationContext context);

        #endregion Methods
    }
}