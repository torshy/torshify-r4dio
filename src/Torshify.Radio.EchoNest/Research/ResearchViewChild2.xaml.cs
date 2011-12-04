using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest.Research
{
    [Export(typeof(ResearchViewChild2))]
    public partial class ResearchViewChild2 : UserControl
    {
        public ResearchViewChild2()
        {
            InitializeComponent();
        }

        [Import]
        public ResearchViewChild2ViewModel Model
        {
            get { return DataContext as ResearchViewChild2ViewModel; }
            set { DataContext = value; }
        }
    }

    [Export(typeof(ResearchViewChild2ViewModel))]
    public class ResearchViewChild2ViewModel : NotificationObject, INavigationAware
    {
        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
