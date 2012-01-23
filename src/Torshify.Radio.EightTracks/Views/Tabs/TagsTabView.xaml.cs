using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(TagsTabView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TagsTabView : UserControl
    {
        public TagsTabView()
        {
            InitializeComponent();
        }

        [Import]
        public TagsTabViewModel Model
        {
            get { return DataContext as TagsTabViewModel; }
            set { DataContext = value; }
        }
    }
}
