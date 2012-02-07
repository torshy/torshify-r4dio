using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.Core.Views.Twitter
{
    [Export]
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class TwitterView : UserControl
    {
        public TwitterView()
        {
            InitializeComponent();
        }

        [Import]
        public TwitterViewModel Model
        {
            get { return DataContext as TwitterViewModel; }
            set { DataContext = value; }
        }  
    }
}
