using EchoNest.Artist;

using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest.Views.Style.Models
{
    public class TermModel : NotificationObject
    {
        #region Fields

        private bool _ban;
        private string _count;
        private bool _require;

        #endregion Fields

        #region Constructors

        public TermModel(string name, ListTermsType type)
        {
            Name = name;
            Type = type;
        }

        #endregion Constructors

        #region Properties

        public string Name
        {
            get;
            private set;
        }

        public ListTermsType Type
        {
            get;
            private set;
        }

        public string Count
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

        public bool Require
        {
            get { return _require; }
            set
            {
                if (_require != value)
                {
                    _require = value;
                    RaisePropertyChanged("Require");
                }
            }
        }

        public bool Ban
        {
            get { return _ban; }
            set
            {
                if (_ban != value)
                {
                    _ban = value;
                    RaisePropertyChanged("Ban");
                }
            }
        }

        #endregion Properties
    }
}