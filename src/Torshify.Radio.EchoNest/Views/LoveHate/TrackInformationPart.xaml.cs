using System.Windows;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.Views.LoveHate
{
    public partial class TrackInformationPart : UserControl
    {
        #region Fields

        public static readonly DependencyProperty AlbumArtProperty = 
            DependencyProperty.Register("AlbumArt", typeof(string), typeof(TrackInformationPart),
                new FrameworkPropertyMetadata((string)null));
        public static readonly DependencyProperty AlbumArtSizeProperty = 
            DependencyProperty.Register("AlbumArtSize", typeof(double), typeof(TrackInformationPart),
                new FrameworkPropertyMetadata((double)200));
        public static readonly DependencyProperty AlbumNameFontSizeProperty = 
            DependencyProperty.Register("AlbumNameFontSize", typeof(double), typeof(TrackInformationPart),
                new FrameworkPropertyMetadata((double)20));
        public static readonly DependencyProperty ArtistNameProperty = 
            DependencyProperty.Register("ArtistName", typeof(string), typeof(TrackInformationPart),
                new FrameworkPropertyMetadata((string)null));
        public static readonly DependencyProperty SubContentProperty = 
            DependencyProperty.Register("SubContent", typeof(object), typeof(TrackInformationPart),
                new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty TrackNameFontSizeProperty = 
            DependencyProperty.Register("TrackNameFontSize", typeof(double), typeof(TrackInformationPart),
                new FrameworkPropertyMetadata((double)30));
        public static readonly DependencyProperty TrackNameProperty = 
            DependencyProperty.Register("TrackName", typeof(string), typeof(TrackInformationPart),
                new FrameworkPropertyMetadata((string)null));

        #endregion Fields

        #region Constructors

        public TrackInformationPart()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public object SubContent
        {
            get
            {
                return (object)GetValue(SubContentProperty);
            }
            set
            {
                SetValue(SubContentProperty, value);
            }
        }

        public double AlbumNameFontSize
        {
            get
            {
                return (double)GetValue(AlbumNameFontSizeProperty);
            }
            set
            {
                SetValue(AlbumNameFontSizeProperty, value);
            }
        }

        public double TrackNameFontSize
        {
            get
            {
                return (double)GetValue(TrackNameFontSizeProperty);
            }
            set
            {
                SetValue(TrackNameFontSizeProperty, value);
            }
        }

        public double AlbumArtSize
        {
            get
            {
                return (double)GetValue(AlbumArtSizeProperty);
            }
            set
            {
                SetValue(AlbumArtSizeProperty, value);
            }
        }

        public string AlbumArt
        {
            get
            {
                return (string)GetValue(AlbumArtProperty);
            }
            set
            {
                SetValue(AlbumArtProperty, value);
            }
        }

        public string ArtistName
        {
            get
            {
                return (string)GetValue(ArtistNameProperty);
            }
            set
            {
                SetValue(ArtistNameProperty, value);
            }
        }

        public string TrackName
        {
            get
            {
                return (string)GetValue(TrackNameProperty);
            }
            set
            {
                SetValue(TrackNameProperty, value);
            }
        }

        #endregion Properties
    }
}