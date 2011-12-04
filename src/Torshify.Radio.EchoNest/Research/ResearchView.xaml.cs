using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.EchoNest.Research
{
    public partial class ResearchView : UserControl
    {
        private readonly IRegionManager _regionManager;

        public ResearchView(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
            RegionManager.SetRegionManager(_contentControl, regionManager); 
            _regionManager.RequestNavigate("ResearchRegion", new Uri("/ResearchViewChild2", UriKind.Relative));
        }
    }
}
