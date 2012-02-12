using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Torshify.Radio.Core.Views.NowPlaying.UI
{
    public partial class UpcomingTracksPart : UserControl
    {
        #region Fields

        public static readonly DependencyProperty UpcomingTracksProperty = 
            DependencyProperty.Register("UpcomingTracks", typeof(IEnumerable), typeof(UpcomingTracksPart),
                new FrameworkPropertyMetadata((IEnumerable)null));
        public static readonly DependencyProperty UpcomingTrackStreamsProperty = 
            DependencyProperty.Register("UpcomingTrackStreams", typeof(IEnumerable), typeof(UpcomingTracksPart),
                new FrameworkPropertyMetadata((IEnumerable)null));

        #endregion Fields

        #region Constructors

        public UpcomingTracksPart()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public IEnumerable UpcomingTrackStreams
        {
            get
            {
                return (IEnumerable)GetValue(UpcomingTrackStreamsProperty);
            }
            set
            {
                SetValue(UpcomingTrackStreamsProperty, value);
            }
        }

        public IEnumerable UpcomingTracks
        {
            get
            {
                return (IEnumerable)GetValue(UpcomingTracksProperty);
            }
            set
            {
                SetValue(UpcomingTracksProperty, value);
            }
        }

        #endregion Properties
    }
}