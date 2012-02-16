using System.Windows;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.Views.Favorites.Tabs
{
    public partial class FavoriteItem : DockPanel
    {
        #region Fields

        public static readonly DependencyProperty DescriptionProperty = 
            DependencyProperty.Register("Description", typeof(string), typeof(FavoriteItem),
                new FrameworkPropertyMetadata((string)null));
        public static readonly DependencyProperty ImageUriProperty = 
            DependencyProperty.Register("ImageUri", typeof(string), typeof(FavoriteItem),
                new FrameworkPropertyMetadata((string)null));
        public static readonly DependencyProperty SubTitleProperty = 
            DependencyProperty.Register("SubTitle", typeof(string), typeof(FavoriteItem),
                new FrameworkPropertyMetadata((string)null));
        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register("Title", typeof(string), typeof(FavoriteItem),
                new FrameworkPropertyMetadata((string)null));

        #endregion Fields

        #region Constructors

        public FavoriteItem()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        public string SubTitle
        {
            get
            {
                return (string)GetValue(SubTitleProperty);
            }
            set
            {
                SetValue(SubTitleProperty, value);
            }
        }

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public string ImageUri
        {
            get
            {
                return (string)GetValue(ImageUriProperty);
            }
            set
            {
                SetValue(ImageUriProperty, value);
            }
        }

        #endregion Properties
    }
}