using System;
using System.ComponentModel.Composition;
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
        private readonly ITrackPlayer _player;

        private ImageSource _backgroundImage;
        private IRegionNavigationService _navigationService;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public NowPlayingViewModel(IRadio radio, [Import("CorePlayer")] ITrackPlayer player)
        {
            _radio = radio;
            _player = player;

            NavigateBackCommand = new AutomaticCommand(ExecuteNavigateBack, CanExecuteNavigateBack);
            NextTrackCommand = new ManualCommand(ExecuteNextTrack, CanExecuteNextTrack);
            TogglePlayPauseCommand = new ManualCommand(ExecuteTogglePlayPause, CanExecuteTogglePlayPause);
            ShareTrackCommand = new ManualCommand<Track>(ExecuteShareTrack, CanExecuteShareTrack);
        }

        #endregion Constructors

        #region Properties

        public ManualCommand<Track> ShareTrackCommand
        {
            get;
            private set;
        }

        public ManualCommand TogglePlayPauseCommand
        {
            get;
            private set;
        }

        public ManualCommand NextTrackCommand
        {
            get;
            private set;
        }

        public ICommand NavigateBackCommand
        {
            get; private set;
        }

        public IRadio Radio
        {
            get { return _radio; }
        }

        public ITrackPlayer Player
        {
            get
            {
                return _player;
            }
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

        private bool _hasTrack;

        public bool HasTrack
        {
            get { return _hasTrack; }
            set
            {
                if (_hasTrack != value)
                {
                    _hasTrack = value;
                    RaisePropertyChanged("HasTrack");
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
                HasTrack = true;
                QueryForBackdrop(_radio.CurrentTrack.Artist);
            }
        }

        private void RadioOnUpcomingTrackChanged(object sender, EventArgs eventArgs)
        {
        }

        private void RadioOnCurrentTrackChanged(object sender, TrackChangedEventArgs e)
        {
            if (e.CurrentTrack == null)
            {
                HasTrack = false;
            }

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
                            BackgroundImage = GetImageSource(imageUrl);
                        }
                        else
                        {
                            string[] randoms;
                            if (BackdropService.TryGetAny(out randoms))
                            {
                                BackgroundImage = GetImageSource(randoms[0]);
                            }
                            else
                            {
                                BackgroundImage = null;
                            }
                        }
                    }
                });
        }

        private static BitmapImage GetImageSource(string imageUrl)
        {
            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.CacheOption = BitmapCacheOption.None;
            imageSource.UriSource = new Uri(imageUrl, UriKind.Absolute);
            imageSource.EndInit();

            if (imageSource.CanFreeze)
            {
                imageSource.Freeze();
            }

            return imageSource;
        }

        private bool CanExecuteNavigateBack()
        {
            return _navigationService != null && _navigationService.Journal.CanGoBack;
        }

        private void ExecuteNavigateBack()
        {
            _navigationService.Journal.GoBack();
        }

        private bool CanExecuteShareTrack(Track track)
        {
            return true;
        }

        private void ExecuteShareTrack(Track track)
        {
        }

        private void ExecuteTogglePlayPause()
        {
            if (_player.IsPlaying)
            {
                _player.Pause();
            }
            else
            {
                _player.Play();
            }
        }

        private bool CanExecuteTogglePlayPause()
        {
            return _radio.CurrentTrack != null;
        }

        private bool CanExecuteNextTrack()
        {
            return _radio.CanGoToNextTrack;
        }

        private void ExecuteNextTrack()
        {
            _radio.NextTrack();
        }

        #endregion Methods
    }
}