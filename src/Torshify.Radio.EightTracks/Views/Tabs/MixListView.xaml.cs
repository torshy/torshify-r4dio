using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    public partial class MixListView : UserControl
    {
        #region Constructors

        public MixListView()
        {
            InitializeComponent();

            MixList.ItemContainerGenerator.ItemsChanged += ItemContainerGeneratorOnItemsChanged;
        }

        #endregion Constructors

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