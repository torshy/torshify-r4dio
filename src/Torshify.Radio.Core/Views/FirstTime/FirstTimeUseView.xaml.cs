using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.Core.Views.FirstTime
{
    [Export(typeof(FirstTimeUseView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class FirstTimeUseView : UserControl
    {
        #region Constructors

        public FirstTimeUseView()
        {
            InitializeComponent();
            Loaded += ViewLoaded;
        }

        #endregion Constructors

        #region Properties

        [Import]
        public FirstTimeUseViewModel Model
        {
            get { return DataContext as FirstTimeUseViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var wizardPage in Model.WizardPages)
            {
                _wizard.Items.Add(wizardPage);
            }

            _wizard.Items.Add(new LastWizardPage());
        }

        private void WizardFinish(object sender, RoutedEventArgs e)
        {
            Model.FinishCommand.Execute(null);
        }

        #endregion Methods
    }
}