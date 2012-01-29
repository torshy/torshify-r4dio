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

        private bool _isPlaying;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public ControlsViewModel(IRadio radio, CorePlayer player)
        {
            _radio = radio;
            _radio.CurrentTrackChanged += RadioOnCurrentTrackChanged;
            _radio.UpcomingTrackChanged += RadioOnUpcomingTrackChanged;
            _player = player;
            _player.IsPlayingChanged += (sender, args) => RaisePropertyChanged("IsPlaying");

            NextTrackCommand = new AutomaticCommand(ExecuteNextTrackCommand, CanExecuteNextTrack);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand NextTrackCommand
        {
            get;
            private set;
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
            get { return _isPlaying; }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    RaisePropertyChanged("IsPlaying");
                }
            }
        }

        #endregion Properties

        #region Methods

        private bool CanExecuteNextTrack()
        {
            return _radio.CanGoToNextTrack;
        }

        private void ExecuteNextTrackCommand()
        {
            _radio.NextTrack();
        }

        private void RadioOnUpcomingTrackChanged(object sender, EventArgs eventArgs)
        {
        }

        private void RadioOnCurrentTrackChanged(object sender, EventArgs eventArgs)
        {
            RaisePropertyChanged("HasTracks");
        }

        #endregion Methods
    }
}