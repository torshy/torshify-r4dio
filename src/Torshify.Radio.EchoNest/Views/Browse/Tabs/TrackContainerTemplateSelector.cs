using System.Windows;
using System.Windows.Controls;

using Torshify.Radio.EchoNest.Views.Browse.Tabs.Models;

namespace Torshify.Radio.EchoNest.Views.Browse.Tabs
{
    public class TrackContainerTemplateSelector : DataTemplateSelector
    {
        #region Properties

        public DataTemplate ArtistInfoTemplate
        {
            get; set;
        }

        public DataTemplate AlbumTemplate
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ArtistInformationContainer)
            {
                return ArtistInfoTemplate;
            }

            return AlbumTemplate;
        }

        #endregion Methods
    }
}