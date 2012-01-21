using System.ComponentModel;
using System.Dynamic;

namespace Torshify.Radio.Framework
{
    public class TrackContainerOwner : INotifyPropertyChanged
    {
        #region Fields

        private string _name;

        #endregion Fields

        #region Constructors

        public TrackContainerOwner()
            : this(null)
        {
        }

        public TrackContainerOwner(string name)
        {
            Name = name;
            ExtraData = new ExpandoObject();
        }
        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public dynamic ExtraData
        {
            get;
            private set;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        #endregion Properties

        #region Methods

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Methods
    }
}