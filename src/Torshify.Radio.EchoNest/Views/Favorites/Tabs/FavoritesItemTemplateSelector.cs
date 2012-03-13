using System.Windows;
using System.Windows.Controls;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Favorites.Tabs
{
    public class FavoritesItemTemplateSelector : DataTemplateSelector
    {
        #region Properties

        public DataTemplate TrackFavorite
        {
            get; set;
        }

        public DataTemplate TrackContainerFavorite
        {
            get; set;
        }

        public DataTemplate TrackStreamFavorite
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is TrackFavorite)
            {
                return TrackFavorite;
            }

            if (item is TrackContainerFavorite)
            {
                return TrackContainerFavorite;
            }

            if (item is TrackStreamFavorite)
            {
                return TrackStreamFavorite;
            }

            return base.SelectTemplate(item, container);
        }

        #endregion Methods
    }
}