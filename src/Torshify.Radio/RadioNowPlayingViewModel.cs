using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio
{
    public class RadioNowPlayingViewModel : NotificationObject
    {
        #region Fields

        private readonly IRadio _radio;
        private readonly IEventAggregator _eventAggregator;
        private Queue<IRadioTrack> _playQueue;
        private TaskScheduler _uiTaskScheduler;
        #endregion Fields

        #region Constructors

        public RadioNowPlayingViewModel(IRadio radio, IEventAggregator eventAggregator)
        {
            _radio = radio;
            _eventAggregator = eventAggregator;
            _radio.TrackComplete += OnTrackComplete;
            _playQueue = new Queue<IRadioTrack>();
            _uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        #endregion Constructors

        #region Events

        public event EventHandler AtEndOfPlaylist;

        #endregion Events

        #region Properties

        public bool HasTracks
        {
            get { return CurrentTrack != null || _playQueue.Count > 0; }
        }

        public bool HasUpNext
        {
            get { return NextTrack != null; }
        }

        public IRadioTrack CurrentTrack
        {
            get;
            private set;
        }

        public IRadioTrack NextTrack
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public void AddTracks(IEnumerable<IRadioTrack> tracks)
        {
            foreach (var radioTrack in tracks)
            {
                _playQueue.Enqueue(radioTrack);
            }
        }

        public void ClearTracks()
        {
            _playQueue.Clear();

            CurrentTrack = null;
            NextTrack = null;

            RaisePropertyChanged("CurrentTrack", "NextTrack", "HasTracks", "HasUpNext");
        }

        public void MoveToNext()
        {
            Task.Factory
                .StartNew(() =>
                {
                    if (_playQueue.Count > 0)
                    {
                        CurrentTrack = _playQueue.Dequeue();
                    }

                    return CurrentTrack;
                })
                .ContinueWith(t =>
                {
                    if (t.Result != null)
                    {
                        _radio.Load(t.Result);
                        _radio.Play();
                    }

                    RaisePropertyChanged("CurrentTrack", "HasTracks");
                }, _uiTaskScheduler)
                .ContinueWith(t =>
                    PeekToNext());
        }

        public void PeekToNext()
        {
            if (_playQueue.Count > 0)
            {
                NextTrack = _playQueue.Peek();
            }
            else
            {
                NextTrack = null;
                OnAtEndAtPlaylist();
            }

            RaisePropertyChanged("NextTrack", "HasUpNext");
        }

        private void OnAtEndAtPlaylist()
        {
            var handler = AtEndOfPlaylist;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnTrackComplete(object sender, TrackEventArgs e)
        {
            MoveToNext();
        }

        #endregion Methods
    }
}