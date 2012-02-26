using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Spotify.Views.Settings
{
    public class SpotifySettingsPageModel : ISettingsPage
    {
        #region Fields

        private ObservableCollection<ISettingsSection> _sections;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public SpotifySettingsPageModel(ILoadingIndicatorService loadingIndicator)
        {
            HeaderInfo = new HeaderInfo();
            HeaderInfo.Title = "Spotify";
            //HeaderInfo.IconUri = "pack://application:,,,/Torshify.Radio.Spotify;component/Resources/Spotify_Logo.png";

            _sections = new ObservableCollection<ISettingsSection>();
            _sections.Add(new SpotifyLoginSection(loadingIndicator));
        }

        #endregion Constructors

        #region Properties

        public HeaderInfo HeaderInfo
        {
            get; private set;
        }

        public IEnumerable<ISettingsSection> Sections
        {
            get { return _sections; }
        }

        #endregion Properties
    }
}