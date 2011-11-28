using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio
{
    public class RadioStationsViewModel : NotificationObject
    {
        #region Constructors

        public RadioStationsViewModel(IRadio radio)
        {
            Radio = radio;
        }

        #endregion Constructors

        #region Properties

        public IRadio Radio
        {
            get; private set;
        }

        #endregion Properties
    }
}