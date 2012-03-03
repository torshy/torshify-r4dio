using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    [Export(typeof(TagsTabView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TagsTabView : UserControl
    {
        #region Constructors

        public TagsTabView()
        {
            InitializeComponent();

            MixList.ItemContainerGenerator.ItemsChanged += ItemContainerGeneratorOnItemsChanged;
        }

        #endregion Constructors

        #region Properties

        [Import]
        public TagsTabViewModel Model
        {
            get { return DataContext as TagsTabViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void ItemContainerGeneratorOnItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                MixListScrollViewer.ScrollToTop();
            }
        }

        #endregion Methods
    }
}