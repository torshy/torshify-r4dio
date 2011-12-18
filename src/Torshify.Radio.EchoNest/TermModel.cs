using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest
{
    public class TermModel : NotificationObject
    {
        #region Fields

        private int _count;

        #endregion Fields

        #region Constructors

        public TermModel()
        {
            Boost = 1.0;
        }

        #endregion Constructors

        #region Properties

        public bool Ban
        {
            get;
            set;
        }

        public double Boost
        {
            get;
            set;
        }

        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                RaisePropertyChanged("Count");
            }
        }

        public string Name
        {
            get;
            set;
        }

        public bool Require
        {
            get;
            set;
        }

        #endregion Properties
    }
}