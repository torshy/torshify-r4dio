using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shell;
using System.Windows.Threading;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.WindowsAPICodePack.Taskbar;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Events;

using JumpList = System.Windows.Shell.JumpList;

namespace Torshify.Radio.Core.Startables
{
    public class TaskbarHandler : IStartable
    {
        #region Fields

        [Import]
        private IEventAggregator _eventAggregator = null;
        private JumpList _jumpList;
        [Import]
        private IRadio _radio = null;
        [Import("CorePlayer")]
        private ITrackPlayer _player = null;
        [Import]
        private IRegionManager _regionManager = null;
        [Import]
        private ITileService _tileService = null;
        [Import]
        private Dispatcher _dispatcher = null;

        private ThumbnailToolBarButton _togglePlayPauseButton;
        private ThumbnailToolBarButton _nextTrackButton;

        #endregion Fields

        #region Methods

        public void Start()
        {
            if (TaskbarManager.IsPlatformSupported)
            {
                _eventAggregator.GetEvent<ApplicationArgumentsEvent>().Subscribe(OnArgumentsReceived, ThreadOption.UIThread, true);
                _jumpList = new JumpList();
                _tileService.Tiles.ForEach(AddJumpTask);
                _tileService.TileAdded += (sender, args) => AddJumpTask(args.Tile);
                JumpList.SetJumpList(Application.Current, _jumpList);

                _radio.CurrentTrackChanged += RadioOnCurrentTrackChanged;
                _player.IsPlayingChanged += PlayerOnIsPlayingChanged;
                _togglePlayPauseButton = new ThumbnailToolBarButton(Properties.Resources.play_circle, "Play");
                _togglePlayPauseButton.Click += TogglePlayPauseButtonOnClick;
                _togglePlayPauseButton.Enabled = false;
                _nextTrackButton = new ThumbnailToolBarButton(Properties.Resources.ff_circle, "Next");
                _nextTrackButton.Click += NextTrackButtonOnClick;
                _nextTrackButton.Enabled = false;

                try
                {
                    TaskbarManager
                        .Instance
                        .ThumbnailToolBars
                        .AddButtons(
                            new WindowInteropHelper(
                                Application.Current.MainWindow).Handle,
                            _togglePlayPauseButton,
                            _nextTrackButton);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private void PlayerOnIsPlayingChanged(object sender, EventArgs e)
        {
            if (_player.IsPlaying)
            {
                _dispatcher.BeginInvoke(new Action(() =>
                {
                    _togglePlayPauseButton.Icon = Properties.Resources.pause_circle;
                }));
            }
            else
            {
                _dispatcher.BeginInvoke(new Action(() =>
                {
                    _togglePlayPauseButton.Icon = Properties.Resources.play_circle;
                }));
            }
        }

        private void RadioOnCurrentTrackChanged(object sender, TrackChangedEventArgs e)
        {
            if (e.CurrentTrack != null)
            {
                _dispatcher.BeginInvoke(new Action(() =>
                {
                    _togglePlayPauseButton.Enabled = true;
                    _nextTrackButton.Enabled = true;
                }));
            }
            else
            {
                _dispatcher.BeginInvoke(new Action(() =>
                {
                    _togglePlayPauseButton.Enabled = false;
                    _nextTrackButton.Enabled = false;
                }));
            }
        }

        private void NextTrackButtonOnClick(object sender, ThumbnailButtonClickedEventArgs e)
        {
            if (_radio.CanGoToNextTrack)
            {
                _radio.NextTrack();
            }
        }

        private void TogglePlayPauseButtonOnClick(object sender, ThumbnailButtonClickedEventArgs e)
        {
            if (AppCommands.TogglePlayCommand.CanExecute(null))
            {
                AppCommands.TogglePlayCommand.Execute(null);
            }
        }

        private void OnArgumentsReceived(IEnumerable<string> args)
        {
            string query;

            if (args.Count() > 1)
            {
                query = args.Skip(1).FirstOrDefault();
            }
            else
            {
                query = args.FirstOrDefault();
            }

            UriQuery q = new UriQuery(query);

            if (!string.IsNullOrEmpty(q["ViewId"]) && !string.IsNullOrEmpty(q["RegionId"]))
            {
                _regionManager.RequestNavigate(q["RegionId"], q["ViewId"]);
            }
        }

        private void AddJumpTask(Tile tile)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action<Tile>(t =>
                {
                    UriQuery q = new UriQuery();
                    q.Add("ViewId", t.NavigationUri.OriginalString);
                    q.Add("RegionId", t.TargetRegionName);

                    JumpTask task = new JumpTask();
                    task.Title = t.Data.Title;
                    task.Arguments = q.ToString();

                    _jumpList.JumpItems.Add(task);
                    _jumpList.Apply();
                }),
                DispatcherPriority.ContextIdle,
                tile);
        }

        #endregion Methods
    }
}