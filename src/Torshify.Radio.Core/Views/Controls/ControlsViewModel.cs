using System;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.Core.Views.Controls
{
    [Export(typeof(ControlsViewModel))]
    public class ControlsViewModel : NotificationObject
    {
        #region Fields

        private readonly CorePlayer _player;
        private readonly IRadio _radio;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public ControlsViewModel(IRadio radio, CorePlayer player)
        {
            _radio = radio;
            _radio.CurrentTrackChanged += RadioOnCurrentTrackChanged;
            _radio.UpcomingTrackChanged += RadioOnUpcomingTrackChanged;
            _radio.CurrentTrackStreamChanged += RadioOnCurrentTrackStreamChanged;
            _player = player;
            _player.IsPlayingChanged += (sender, args) => RaisePropertyChanged("IsPlaying");

            NextTrackCommand = new AutomaticCommand(ExecuteNextTrack, CanExecuteNextTrack);
            TogglePlayPauseCommand = new AutomaticCommand(ExecuteTogglePlayPause, CanExecuteTogglePlayPause);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand TogglePlayPauseCommand
        {
            get;
            private set;
        }

        public AutomaticCommand NextTrackCommand
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

        public double Volume
        {
            get
            {
                return _player.Volume;
            }
            set
            {
                _player.Volume = value;
            }
        }

        #endregion Properties

        #region Methods

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
        }

        private void RadioOnUpcomingTrackChanged(object sender, EventArgs eventArgs)
        {
            RaisePropertyChanged("HasUpcomingTrack", "UpcomingTrack");
        }

        private void RadioOnCurrentTrackChanged(object sender, EventArgs eventArgs)
        {
            RaisePropertyChanged("CurrentTrack", "HasTracks", "Volume");
        }

        #endregion Methods
    }
}