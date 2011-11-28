using System.Windows;
using System.Windows.Controls.Primitives;

namespace Torshify.Radio.Controls
{
    [TemplatePart(Name = "PART_MinimizeButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_MaximizeButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(ButtonBase))]
    public class MetroWindow : Window
    {
        #region Fields

        private WindowState _lastWindowState;
        private ButtonBase _partCloseButton;
        private ButtonBase _partMaximizeButton;
        private ButtonBase _partMinimizeButton;

        #endregion Fields

        #region Constructors

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        #endregion Constructors

        #region Methods

        public override void OnApplyTemplate()
        {
            // Close Button
            ButtonBase oldCloseButton = _partCloseButton;
            _partCloseButton = Template.FindName("PART_CloseButton", this) as ButtonBase;

            if (!ReferenceEquals(oldCloseButton, _partCloseButton))
            {
                if (oldCloseButton != null)
                {
                    oldCloseButton.Click -= OnCloseButtonClick;
                }

                if (_partCloseButton != null)
                {
                    _partCloseButton.Click += OnCloseButtonClick;
                }
            }

            // Minimize Button
            ButtonBase oldMinimizeButton = _partMinimizeButton;
            _partMinimizeButton = Template.FindName("PART_MinimizeButton", this) as ButtonBase;

            if (!ReferenceEquals(oldMinimizeButton, _partMinimizeButton))
            {
                if (oldMinimizeButton != null)
                {
                    oldMinimizeButton.Click -= OnMinimizeButtonClick;
                }

                if (_partMinimizeButton != null)
                {
                    _partMinimizeButton.Click += OnMinimizeButtonClick;
                }
            }

            // Maximize Button
            ButtonBase oldMaximizeButton = _partMaximizeButton;
            _partMaximizeButton = Template.FindName("PART_MaximizeButton", this) as ButtonBase;

            if (!ReferenceEquals(oldMaximizeButton, _partMaximizeButton))
            {
                if (oldMaximizeButton != null)
                {
                    oldMaximizeButton.Click -= OnMaximizeButtonClick;
                }

                if (_partMaximizeButton != null)
                {
                    _partMaximizeButton.Click += OnMaximizeButtonClick;
                }
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnMaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = _lastWindowState;
            }
            else
            {
                _lastWindowState = WindowState;
                WindowState = WindowState.Maximized;
            }
        }

        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion Methods
    }
}