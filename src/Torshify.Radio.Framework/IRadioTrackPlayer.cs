using System;

namespace Torshify.Radio.Framework
{
    public interface IRadioTrackPlayer
    {
        #region Events

        event EventHandler IsPlayingChanged;

        event EventHandler<TrackEventArgs> TrackComplete;

        event EventHandler<TrackProgressEventArgs> TrackProgress;

        #endregion Events

        #region Properties

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

        bool CanPlay(IRadioTrack track);

        void Load(IRadioTrack track);

        void Stop();

        void Play();

        void Pause();

        #endregion Methods
    }
}