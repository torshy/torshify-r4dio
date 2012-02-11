using System;
using System.ComponentModel.Composition;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.Core.Views.Controls
{
    [Export(typeof(ControlsViewModel))]
    public class ControlsViewModel : NotificationObject
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private readonly ITrackPlayer _player;
        private readonly IRadio _radio;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public ControlsViewModel(IRadio radio, [Import("CorePlayer")] ITrackPlayer player, Dispatcher dispatcher)
        {
            _radio = radio;
            _radio.CurrentTrackChanged += RadioOnCurrentTrackChanged;
            _radio.UpcomingTrackChanged += RadioOnUpcomingTrackChanged;
            _radio.CurrentTrackStreamChanged += RadioOnCurrentTrackStreamChanged;
            _player = player;
            _dispatcher = dispatcher;
            _player.IsPlayingChanged += (sender, args) =>
                                        {
                                            RaisePropertyChanged("IsPlaying");
                                            RefreshCommands();
                                        };

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

        public ITrackStream CurrentTrackStream
        {
            get
            {
                return _radio.CurrentTrackStream;
            }
        }

        public Track CurrentTrack
        {
            get
            {
                return _radio.CurrentTrack;
            }
        }

        public Track UpcomingTrack
        {
            get
            {
                return _radio.UpcomingTrack;
            }
        }

        public bool HasUpcomingTrack
        {
            get
            {
                return _radio.UpcomingTrack != null;
            }
        }

        public bool HasTracks
        {
            get
            {
                return _radio.CurrentTrack != null;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _player.IsPlaying;
            }
        }

        public ITrackPlayer Player
        {
            get
            {
                return _player;
            }
        }

        #endregion Properties

        #region Methods

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

        private void RadioOnCurrentTrackStreamChanged(object sender, EventArgs eventArgs)
        {
            RaisePropertyChanged("CurrentTrackStream");
            RefreshCommands();
        }

        private void RadioOnUpcomingTrackChanged(object sender, EventArgs eventArgs)
        {
            RaisePropertyChanged("HasUpcomingTrack", "UpcomingTrack");
            RefreshCommands();
        }

        private void RadioOnCurrentTrackChanged(object sender, EventArgs eventArgs)
        {
            RaisePropertyChanged("CurrentTrack", "HasTracks", "Volume");
            RefreshCommands();
        }

        private void RefreshCommands()
        {
            if (_dispatcher.CheckAccess())
            {
                TogglePlayPauseCommand.NotifyCanExecuteChanged();
                NextTrackCommand.NotifyCanExecuteChanged();
            }
            else
            {
                _dispatcher.BeginInvoke(new Action(RefreshCommands));
            }
        }

        #endregion Methods
    }
}