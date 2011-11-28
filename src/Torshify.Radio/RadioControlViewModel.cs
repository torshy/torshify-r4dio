using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio
{
    public class RadioControlViewModel : NotificationObject
    {
        #region Fields

        private readonly IRadio _radio;

        #endregion Fields

        #region Constructors

        public RadioControlViewModel(IRadio radio)
        {
            _radio = radio;
        }

        #endregion Constructors

        public IRadio Radio
        {
            get
            {
                return _radio;
            }
        }
    }
}