using System;

using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest.Views.Style.Models
{
    public class Range : NotificationObject
    {
        #region Fields

        private double? _maximum;
        private double? _minimum;

        #endregion Fields

        #region Events

        public event EventHandler RangeChanged;

        #endregion Events

        #region Properties

        public double? Minimum
        {
            get { return _minimum; }
            set
            {
                if (_minimum != value)
                {
                    if (value.HasValue)
                    {
                        _minimum = Math.Round(value.Value, 1, Rounding);
                    }
                    else
                    {
                        _minimum = null;
                    }

                    RaisePropertyChanged("Minimum");
                    OnRangeChanged();
                }
            }
        }

        public double? Maximum
        {
            get { return _maximum; }
            set
            {
                if (_maximum != value)
                {
                    if (value.HasValue)
                    {
                        _maximum = Math.Round(value.Value, 1, Rounding);
                    }
                    else
                    {
                        _maximum = null;
                    }

                    RaisePropertyChanged("Maximum");
                    OnRangeChanged();
                }
            }
        }

        public MidpointRounding Rounding
        {
            get; 
            set;
        }

        #endregion Properties

        #region Methods

        private void OnRangeChanged()
        {
            var handler = RangeChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}