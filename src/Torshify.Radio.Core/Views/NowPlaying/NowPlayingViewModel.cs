using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;
using System.Linq;

namespace Torshify.Radio.Core.Views.NowPlaying
{
    [Export(typeof(NowPlayingViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public class NowPlayingViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IRadio _radio;

        private ImageSource _backgroundImage;
        private IRegionNavigationService _navigationService;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public NowPlayingViewModel(IRadio radio)
        {
            _radio = radio;

            NavigateBackCommand = new AutomaticCommand(ExecuteNavigateBack, CanExecuteNavigateBack);
        }

        #endregion Constructors

        #region Properties

        public ICommand NavigateBackCommand
        {
            get; private set;
        }

        public IRadio Radio
        {
            get { return _radio; }
        }

        [Import]
        public IBackdropService BackdropService
        {
            get; 
            set;
        }

        [Import]
        public ILoggerFacade Logger
        {
            get;
            set;
        }

        public ImageSource BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                if (_backgroundImage != value)
                {
                    _backgroundImage = value;
                    RaisePropertyChanged("BackgroundImage");
                }
            }
        }

        #endregion Properties

        #region Methods

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            _radio.CurrentTrackChanged -= RadioOnCurrentTrackChanged;
            _radio.UpcomingTrackChanged -= RadioOnUpcomingTrackChanged;
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            _radio.CurrentTrackChanged += RadioOnCurrentTrackChanged;
            _radio.UpcomingTrackChanged += RadioOnUpcomingTrackChanged;

            if (_radio.CurrentTrack != null)
            {
                QueryForBackdrop(_radio.CurrentTrack.Artist);
            }
        }

        private void RadioOnUpcomingTrackChanged(object sender, EventArgs eventArgs)
        {
        }

        private void RadioOnCurrentTrackChanged(object sender, TrackChangedEventArgs e)
        {
            if (e.CurrentTrack != null && e.PreviousTrack != null && e.CurrentTrack.Artist == e.PreviousTrack.Artist)
            {
                return;
            }

            if (e.CurrentTrack != null)
            {
                QueryForBackdrop(e.CurrentTrack.Artist);
            }
        }

        private void QueryForBackdrop(string artist)
        {
            BackdropService
                .Query(artist)
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                    }
                    else
                    {
                        var imageUrl = task.Result.OrderBy(k => Guid.NewGuid()).FirstOrDefault();

                        if (imageUrl != null)
                        {
                            var coverArtSource = new BitmapImage();
                            coverArtSource.BeginInit();
                            coverArtSource.CacheOption = BitmapCacheOption.None;
                            coverArtSource.UriSource = new Uri(imageUrl, UriKind.Absolute);
                            coverArtSource.EndInit();

                            if (coverArtSource.CanFreeze)
                            {
                                coverArtSource.Freeze();
                            }

                            BackgroundImage = coverArtSource;
                        }
                        else
                        {
                            BackgroundImage = null;
                        }
                    }
                });
        }

        private bool CanExecuteNavigateBack()
        {
            return _navigationService != null && _navigationService.Journal.CanGoBack;
        }

        private void ExecuteNavigateBack()
        {
            _navigationService.Journal.GoBack();
        }

        #endregion Methods
    }
}