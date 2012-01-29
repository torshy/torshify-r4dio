using System;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

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
        }

        #endregion Constructors

        #region Properties

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

        private void RadioOnUpcomingTrackChanged(object sender, EventArgs eventArgs)
        {
        }

        private void RadioOnCurrentTrackChanged(object sender, EventArgs eventArgs)
        {
        }

        #endregion Methods
    }
}