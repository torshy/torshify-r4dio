using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Views.Controls
{
    [Export(typeof(ControlsViewModel))]
    public class ControlsViewModel : NotificationObject
    {
        private readonly IRadio _radio;
        private readonly CorePlayer _player;
        private bool _isPlaying;

        [ImportingConstructor]
        public ControlsViewModel(IRadio radio, CorePlayer player)
        {
            _radio = radio;
            _player = player;
            _player.IsPlayingChanged += (sender, args) => RaisePropertyChanged("IsPlaying");
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
    }
}