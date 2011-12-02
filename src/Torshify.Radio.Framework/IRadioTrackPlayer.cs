using System;

namespace Torshify.Radio.Framework
{
    public interface IRadioTrackPlayer
    {
        #region Events

        event EventHandler IsPlayingChanged;

        event EventHandler IsBufferingChanged;

        event EventHandler<TrackEventArgs> TrackComplete;

        event EventHandler<TrackProgressEventArgs> TrackProgress;

        #endregion Events

        #region Properties

        bool IsBuffering
        {
            get;
        }

        bool IsPlaying
        {
            get;
        }

        float Volume
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        void Initialize();

        bool CanPlay(RadioTrack track);

        void Load(RadioTrack track);

        void Stop();

        void Play();

        void Pause();

        #endregion Methods
    }
}