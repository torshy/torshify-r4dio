using System.Windows.Controls;

using Torshify.Radio.Framework.Controls;

namespace Torshify.Radio.Core.Views.Settings.General
{
    public partial class TrackSourceSectionView : UserControl
    {
        #region Constructors

        public TrackSourceSectionView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void ListBoxReorderRequested(object sender, ReorderEventArgs e)
        {
            var model = DataContext as TrackSourceSection;
            var reorderListBox = (ReorderListBox)e.OriginalSource;
            var dragginItem = (string)reorderListBox.ItemContainerGenerator.ItemFromContainer(e.ItemContainer);
            var toItem = (string)reorderListBox.ItemContainerGenerator.ItemFromContainer(e.ToContainer);

            if (model != null && !string.IsNullOrEmpty(dragginItem) && !string.IsNullOrEmpty(toItem))
            {
                model.ChangeTrackSourcePriority(dragginItem, toItem);
            }
        }

        #endregion Methods
    }
}