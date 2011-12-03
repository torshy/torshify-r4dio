using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio
{
    [Export(typeof(MainWindowViewModel))]
    public class MainWindowViewModel : NotificationObject, IPartImportsSatisfiedNotification
    {
        #region Fields

        [ImportMany]
        private IEnumerable<IStartable> _startables = null;
        private MainWindow _view;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public MainWindowViewModel(IRadio radio)
        {
            Radio = radio;

            GlobalCommands.TogglePlayPauseCommand.RegisterCommand(new DelegateCommand(ExecuteTogglePlayPause));
            GlobalCommands.VolumeUpCommand.RegisterCommand(new DelegateCommand(ExecuteVolumeUp));
            GlobalCommands.VolumeDownCommand.RegisterCommand(new DelegateCommand(ExecuteVolumeDown));
            GlobalCommands.TuneInStationCommand.RegisterCommand(new DelegateCommand<Lazy<IRadioStation, IRadioStationMetadata>>(Radio.TuneIn));
            GlobalCommands.ToggleDebugWindowCommand.RegisterCommand(new DelegateCommand(ExecuteToggleDebugWindow));
        }

        #endregion Constructors

        #region Properties

        public IRadio Radio
        {
            get;
            private set;
        }

        public MainWindow View
        {
            get { return _view; }
            set
            {
                _view = value;
                _view.InputBindings.Add(
                    new KeyBinding
                    {
                        Command = GlobalCommands.PlayCommand,
                        Gesture = new KeyGesture(Key.Play)
                    });
                _view.InputBindings.Add(
                    new KeyBinding
                    {
                        Command = GlobalCommands.PauseCommand,
                        Gesture = new KeyGesture(Key.Pause)
                    });
                _view.InputBindings.Add(
                    new KeyBinding
                    {
                        Command = GlobalCommands.StopCommand,
                        Gesture = new KeyGesture(Key.MediaStop)
                    });
                _view.InputBindings.Add(
                    new KeyBinding
                    {
                        Command = GlobalCommands.NextCommand,
                        Gesture = new KeyGesture(Key.MediaNextTrack)
                    });
                _view.InputBindings.Add(
                    new KeyBinding
                    {
                        Command = GlobalCommands.TogglePlayPauseCommand,
                        Gesture = new KeyGesture(Key.MediaPlayPause)
                    });
                _view.InputBindings.Add(
                    new KeyBinding
                    {
                        Command = GlobalCommands.TogglePlayPauseCommand,
                        Gesture = new KeyGesture(Key.Space)
                    });
                _view.InputBindings.Add(
                    new KeyBinding
                    {
                        Command = GlobalCommands.VolumeUpCommand,
                        Gesture = new KeyGesture(Key.VolumeUp)
                    });
                _view.InputBindings.Add(
                    new KeyBinding
                    {
                        Command = GlobalCommands.VolumeUpCommand,
                        Gesture = new KeyGesture(Key.Up)
                    });
                _view.InputBindings.Add(
                    new KeyBinding
                    {
                        Command = GlobalCommands.VolumeDownCommand,
                        Gesture = new KeyGesture(Key.VolumeDown)
                    });
                _view.InputBindings.Add(
                    new KeyBinding
                    {
                        Command = GlobalCommands.VolumeDownCommand,
                        Gesture = new KeyGesture(Key.Down)
                    });
                _view.InputBindings.Add(
                    new KeyBinding
                    {
                        Command = GlobalCommands.ToggleDebugWindowCommand,
                        Gesture = new KeyGesture(Key.D0, ModifierKeys.Alt)
                    });

            }
        }

        #endregion Properties

        #region Methods

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var startable in _startables)
            {
                startable.Start();
            }
        }

        private void ExecuteToggleDebugWindow()
        {
            ConsoleManager.Toggle();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("...torshify debug window...");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void ExecuteTogglePlayPause()
        {
            if (Radio.IsPlaying)
            {
                Radio.Pause();
            }
            else if (Radio.CurrentTrack != null)
            {
                Radio.Play();
            }
        }

        private void ExecuteVolumeDown()
        {
            Radio.Volume = Math.Max(0.0f, Radio.Volume - 0.1f);
        }

        private void ExecuteVolumeUp()
        {
            Radio.Volume = Math.Min(1.0f, Radio.Volume + 0.1f);
        }

        #endregion Methods
    }
}