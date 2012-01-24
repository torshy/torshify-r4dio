using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Torshify.Radio.Core.Views
{
    [Export(typeof(MainView))]
    public partial class MainView : UserControl
    {
        #region Fields

        private readonly DispatcherTimer _deferredAutoCompleteTimer;

        #endregion Fields

        #region Constructors

        public MainView()
        {
            InitializeComponent();

            _deferredAutoCompleteTimer = new DispatcherTimer();
            _deferredAutoCompleteTimer.Tick += OnDeferredAutoCompleteTick;
            _deferredAutoCompleteTimer.Interval = TimeSpan.FromMilliseconds(750);
        }

        #endregion Constructors

        #region Properties

        [Import]
        public MainViewModel Model
        {
            get { return DataContext as MainViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void AutoCompleteTextChanged(object sender, RoutedEventArgs e)
        {
            _deferredAutoCompleteTimer.Stop();
            _deferredAutoCompleteTimer.Start();
        }

        private void OnDeferredAutoCompleteTick(object sender, EventArgs e)
        {
            _deferredAutoCompleteTimer.Stop();
            Model.UpdateAutoCompleteList(InputBox.Text);
        }

        private void SearchTextBoxSearch(object sender, RoutedEventArgs e)
        {
            if (Model.SearchCommand.CanExecute(InputBox.Text))
            {
                Model.SearchCommand.Execute(InputBox.Text);
            }
        }

        #endregion Methods
    }
}