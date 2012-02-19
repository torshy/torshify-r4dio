using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.EchoNest.Views.LoveHate
{
    [Export]
    [RegionMemberLifetime(KeepAlive = false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoveHateView : UserControl
    {
        #region Constructors

        public LoveHateView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public LoveHateViewModel Model
        {
            get
            {
                return DataContext as LoveHateViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion Properties
    }
}