using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.Views.Similar
{
    [Export(typeof(SimilarView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SimilarView : UserControl
    {
        public SimilarView()
        {
            InitializeComponent();
        }

        [Import]
        public SimilarViewModel Model
        {
            get { return DataContext as SimilarViewModel; }
            set { DataContext = value; }
        }  
    }
}
