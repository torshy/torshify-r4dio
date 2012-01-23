using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace Torshify.Radio.Framework.Controls
{
    #region Enumerations

    public enum SearchMode
    {
        Instant,
        Delayed,
    }

    #endregion Enumerations

    [TemplatePart(Name = "PART_SearchIconButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_SearchIconBorder", Type = typeof(Border))]
    public class SearchTextBox : WatermarkAutoCompleteBox
    {
        #region Fields

        public static readonly RoutedEvent SearchEvent = 
            EventManager.RegisterRoutedEvent(
                "Search",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(SearchTextBox));

        private static DependencyPropertyKey HasTextPropertyKey = 
            DependencyProperty.RegisterReadOnly(
                "HasText",
                typeof(bool),
                typeof(SearchTextBox),
                new PropertyMetadata());
        public static DependencyProperty HasTextProperty;
        private static DependencyPropertyKey IsMouseLeftButtonDownPropertyKey = 
            DependencyProperty.RegisterReadOnly(
                "IsMouseLeftButtonDown",
                typeof(bool),
                typeof(SearchTextBox),
                new PropertyMetadata());
        public static DependencyProperty IsMouseLeftButtonDownProperty;
        public static DependencyProperty SearchEventTimeDelayProperty = 
            DependencyProperty.Register(
                "SearchEventTimeDelay",
                typeof(Duration),
                typeof(SearchTextBox),
                new FrameworkPropertyMetadata(
                    new Duration(new TimeSpan(0, 0, 0, 0, 500)),
                    OnSearchEventTimeDelayChanged));
        public static DependencyProperty SearchModeProperty = 
            DependencyProperty.Register(
                "SearchMode",
                typeof(SearchMode),
                typeof(SearchTextBox),
                new PropertyMetadata(SearchMode.Instant));

        private readonly DispatcherTimer _searchEventDelayTimer;

        #endregion Fields

        #region Constructors

        static SearchTextBox()
        {
            HasTextProperty = HasTextPropertyKey.DependencyProperty;
            IsMouseLeftButtonDownProperty = IsMouseLeftButtonDownPropertyKey.DependencyProperty;
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SearchTextBox),
                new FrameworkPropertyMetadata(typeof(SearchTextBox)));
        }

        public SearchTextBox()
        {
            _searchEventDelayTimer = new DispatcherTimer();
            _searchEventDelayTimer.Interval = SearchEventTimeDelay.TimeSpan;
            _searchEventDelayTimer.Tick += OnSeachEventDelayTimerTick;
        }

        #endregion Constructors

        #region Events

        public event RoutedEventHandler Search
        {
            add { AddHandler(SearchEvent, value); }
            remove { RemoveHandler(SearchEvent, value); }
        }

        #endregion Events

        #region Properties

        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            private set { SetValue(HasTextPropertyKey, value); }
        }

        public bool IsMouseLeftButtonDown
        {
            get { return (bool)GetValue(IsMouseLeftButtonDownProperty); }
            private set { SetValue(IsMouseLeftButtonDownPropertyKey, value); }
        }

        public Duration SearchEventTimeDelay
        {
            get { return (Duration)GetValue(SearchEventTimeDelayProperty); }
            set { SetValue(SearchEventTimeDelayProperty, value); }
        }

        public SearchMode SearchMode
        {
            get { return (SearchMode)GetValue(SearchModeProperty); }
            set { SetValue(SearchModeProperty, value); }
        }

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ButtonBase iconButton = Template.FindName("PART_SearchIconButton", this) as ButtonBase;
            if (iconButton != null)
            {
                iconButton.Click += IconButtonOnClick;
            }

            Border iconBorder = Template.FindName("PART_SearchIconBorder", this) as Border;
            if (iconBorder != null) 
            {
                iconBorder.MouseLeftButtonDown += IconBorderMouseLeftButtonDown;
                iconBorder.MouseLeftButtonUp += IconBorderMouseLeftButtonUp;
                iconBorder.MouseLeave += IconBorderMouseLeave;
            }
        }

        private void IconButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (HasText)
            {
                RaiseSearchEvent();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && SearchMode == SearchMode.Instant)
            {
                SetCurrentValue(TextProperty, string.Empty);
            }
            else if ((e.Key == Key.Return || e.Key == Key.Enter) && SearchMode == SearchMode.Delayed) 
            {
                RaiseSearchEvent();
            }
            else 
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnTextChanged(RoutedEventArgs e)
        {
            base.OnTextChanged(e);

            HasText = Text.Length != 0;

            if (SearchMode == SearchMode.Instant)
            {
                _searchEventDelayTimer.Stop();
                _searchEventDelayTimer.Start();
            }
        }

        private static void OnSearchEventTimeDelayChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SearchTextBox stb = o as SearchTextBox;
            if (stb != null)
            {
                stb._searchEventDelayTimer.Interval = ((Duration)e.NewValue).TimeSpan;
                stb._searchEventDelayTimer.Stop();
            }
        }

        private void IconBorderMouseLeave(object obj, MouseEventArgs e)
        {
            IsMouseLeftButtonDown = false;
        }

        private void IconBorderMouseLeftButtonDown(object obj, MouseButtonEventArgs e)
        {
            IsMouseLeftButtonDown = true;
        }

        private void IconBorderMouseLeftButtonUp(object obj, MouseButtonEventArgs e)
        {
            if (!IsMouseLeftButtonDown) return;

            if (HasText && SearchMode == SearchMode.Instant)
            {
                SetCurrentValue(TextProperty, string.Empty);
            }

            if (HasText && SearchMode == SearchMode.Delayed) 
            {
                RaiseSearchEvent();
            }

            IsMouseLeftButtonDown = false;
        }

        private void OnSeachEventDelayTimerTick(object o, EventArgs e)
        {
            _searchEventDelayTimer.Stop();
            RaiseSearchEvent();
        }

        private void RaiseSearchEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(SearchEvent);
            RaiseEvent(args);
        }

        #endregion Methods
    }
}