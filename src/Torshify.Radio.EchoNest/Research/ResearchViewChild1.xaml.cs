using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest.Research
{
    [Export(typeof(ResearchViewChild1))]
    public partial class ResearchViewChild1 : UserControl
    {
        public ResearchViewChild1()
        {
            InitializeComponent();
        }

        [Import]
        public ResearchViewChild1ViewModel Model
        {
            get { return DataContext as ResearchViewChild1ViewModel; }
            set { DataContext = value; }
        }
    }

    [Export(typeof(ResearchViewChild1ViewModel))]
    public class ResearchViewChild1ViewModel : NotificationObject, INavigationAware
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
