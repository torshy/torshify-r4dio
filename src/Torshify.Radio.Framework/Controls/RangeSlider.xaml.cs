using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Torshify.Radio.Framework.Controls
{
    public partial class RangeSlider : UserControl
    {
        #region Fields

        public static readonly DependencyProperty IsLowerSliderEnabledProperty = 
            DependencyProperty.Register("IsLowerSliderEnabled", typeof(bool), typeof(RangeSlider), new UIPropertyMetadata(true));
        public static readonly DependencyProperty IsLowerValueLockedToMinProperty = 
            DependencyProperty.Register("IsLowerValueLockedToMin", typeof(bool), typeof(RangeSlider), new UIPropertyMetadata(false, OnIsLowerValueLockedToMinChanged));

        public static readonly DependencyProperty IsUpperSliderEnabledProperty = 
            DependencyProperty.Register("IsUpperSliderEnabled", typeof(bool), typeof(RangeSlider), new UIPropertyMetadata(true));
        public static readonly DependencyProperty IsUpperValueLockedToMaxProperty = 
            DependencyProperty.Register("IsUpperValueLockedToMax", typeof(bool), typeof(RangeSlider), new UIPropertyMetadata(false, OnIsUpperValueLockedToMaxChanged));

        public static readonly DependencyProperty LargeChangeProperty = 
            DependencyProperty.Register("LargeChange", typeof(double), typeof(RangeSlider),
                    new UIPropertyMetadata(10.0, OnLargeChangePropertyChanged));
        public static readonly RoutedEvent LowerValueChangedEvent = EventManager.RegisterRoutedEvent("LowerValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(RangeSlider));
        public static readonly DependencyProperty LowerValueProperty = 
            DependencyProperty.Register("LowerValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(10.0));
        public static readonly DependencyProperty MaximumProperty = 
            DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(100.0, OnMaximumChanged));
        public static readonly DependencyProperty MinimumProperty = 
            DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0.0));

        public static readonly DependencyProperty SmallChangeProperty = 
            DependencyProperty.Register("SmallChange", typeof(double), typeof(RangeSlider),
                new UIPropertyMetadata(1.0, OnSmallChangePropertyChanged));
        public static readonly RoutedEvent UpperValueChangedEvent = EventManager.RegisterRoutedEvent("UpperValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(RangeSlider));
        public static readonly DependencyProperty UpperValueProperty = 
            DependencyProperty.Register("UpperValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(90.0, OnUpperValueChanged));

        #endregion Fields

        #region Constructors

        public RangeSlider()
        {
            InitializeComponent();

            this.Loaded += Slider_Loaded;
        }

        #endregion Constructors

        #region Events

        public event RoutedPropertyChangedEventHandler<double> LowerValueChanged
        {
            add { AddHandler(LowerValueChangedEvent, value); }
            remove { RemoveHandler(LowerValueChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<double> UpperValueChanged
        {
            add { AddHandler(UpperValueChangedEvent, value); }
            remove { RemoveHandler(UpperValueChangedEvent, value); }
        }

        #endregion Events

        #region Properties

        public bool IsLowerSliderEnabled
        {
            get { return (bool)GetValue(IsLowerSliderEnabledProperty); }
            set { SetValue(IsLowerSliderEnabledProperty, value); }
        }

        public bool IsLowerValueLockedToMin
        {
            get { return (bool)GetValue(IsLowerValueLockedToMinProperty); }
            set { SetValue(IsLowerValueLockedToMinProperty, value); }
        }

        public bool IsUpperSliderEnabled
        {
            get { return (bool)GetValue(IsUpperSliderEnabledProperty); }
            set { SetValue(IsUpperSliderEnabledProperty, value); }
        }

        public bool IsUpperValueLockedToMax
        {
            get { return (bool)GetValue(IsUpperValueLockedToMaxProperty); }
            set { SetValue(IsUpperValueLockedToMaxProperty, value); }
        }

        public double LargeChange
        {
            get { return (double)GetValue(LargeChangeProperty); }
            set { SetValue(LargeChangeProperty, value); }
        }

        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        #endregion Properties

        #region Methods

        public static void OnIsLowerValueLockedToMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider slider = (RangeSlider)d;

            if ((bool)e.NewValue)
            {
                slider.LowerSlider.SetCurrentValue(RangeBase.ValueProperty, slider.LowerSlider.Minimum);
                slider.IsLowerSliderEnabled = false;
            }
            else
            {
                slider.IsLowerSliderEnabled = true;
            }
        }

        public static void OnIsUpperValueLockedToMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider slider = (RangeSlider)d;

            if ((bool)e.NewValue)
            {
                slider.UpperSlider.SetCurrentValue(RangeBase.ValueProperty, slider.UpperSlider.Maximum);
                slider.IsUpperSliderEnabled = false;
            }
            else
            {
                slider.IsUpperSliderEnabled = true;
            }
        }

        public static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider slider = (RangeSlider)d;

            if (slider.IsUpperValueLockedToMax)
            {
                slider.SetCurrentValue(UpperValueProperty, (double) e.NewValue);
            }
        }

        public static void OnUpperValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        protected static void OnLargeChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.NewValue);
        }

        protected static void OnSmallChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.NewValue);
        }

        protected void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            LowerSlider.ValueChanged += LowerSlider_ValueChanged;
            UpperSlider.ValueChanged += UpperSlider_ValueChanged;
        }

        private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsUpperSliderEnabled)
            {
                UpperSlider.SetCurrentValue(RangeBase.ValueProperty, Math.Max(UpperSlider.Value, LowerSlider.Value));

                var _upperValue = UpperSlider.Value;
                var _lowerValue = LowerSlider.Value;

                if (UpperValue > _lowerValue)
                {
                    SetCurrentValue(LowerValueProperty, _lowerValue);
                    SetCurrentValue(UpperValueProperty, _upperValue);
                }
                else
                {
                    SetCurrentValue(UpperValueProperty, _upperValue);
                    SetCurrentValue(LowerValueProperty, _lowerValue);
                }
            }
            else
            {
                var _lowerValue = e.NewValue;

                if (_lowerValue > UpperValue)
                {
                    LowerSlider.SetCurrentValue(RangeBase.ValueProperty, e.OldValue);
                    SetCurrentValue(LowerValueProperty, LowerSlider.Value);
                }
                else
                {
                    SetCurrentValue(LowerValueProperty, _lowerValue);
                }
            }
        }

        private void UpperSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LowerSlider.SetCurrentValue(RangeBase.ValueProperty,Math.Max(Math.Min(UpperSlider.Value, LowerSlider.Value), Minimum));

            var _upperValue = UpperSlider.Value;
            var _lowerValue = LowerSlider.Value;

            if (UpperValue > _lowerValue)
            {
                SetCurrentValue(LowerValueProperty, _lowerValue);
                SetCurrentValue(UpperValueProperty, _upperValue);
            }
            else
            {
                SetCurrentValue(UpperValueProperty, _upperValue);
                SetCurrentValue(LowerValueProperty, _lowerValue);
            }
        }

        #endregion Methods
    }
}