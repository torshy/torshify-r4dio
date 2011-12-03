using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Torshify.Radio
{
    public partial class LoadingScreen : UserControl
    {
        #region Fields

        public static readonly DependencyProperty CancelCommandProperty = 
            DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(LoadingScreen),
                new FrameworkPropertyMetadata((ICommand)null));

        #endregion Fields

        #region Constructors

        public LoadingScreen()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        #endregion Properties
    }
}