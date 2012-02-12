using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Torshify.Radio.Core.Views.NowPlaying.UI
{
    public partial class ControlsPart : UserControl
    {
        #region Fields

        public static readonly DependencyProperty IsPlayingProperty = 
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(ControlsPart),
                new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty NextTrackCommandProperty = 
            DependencyProperty.Register("NextTrackCommand", typeof(ICommand), typeof(ControlsPart),
                new FrameworkPropertyMetadata((ICommand)null));
        public static readonly DependencyProperty TogglePlayPauseCommandProperty = 
            DependencyProperty.Register("TogglePlayPauseCommand", typeof(ICommand), typeof(ControlsPart),
                new FrameworkPropertyMetadata((ICommand)null));

        #endregion Fields

        #region Constructors

        public ControlsPart()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public ICommand NextTrackCommand
        {
            get
            {
                return (ICommand)GetValue(NextTrackCommandProperty);
            }
            set
            {
                SetValue(NextTrackCommandProperty, value);
            }
        }

        public ICommand TogglePlayPauseCommand
        {
            get
            {
                return (ICommand)GetValue(TogglePlayPauseCommandProperty);
            }
            set
            {
                SetValue(TogglePlayPauseCommandProperty, value);
            }
        }

        public bool IsPlaying
        {
            get
            {
                return (bool)GetValue(IsPlayingProperty);
            }
            set
            {
                SetValue(IsPlayingProperty, value);
            }
        }

        #endregion Properties
    }
}