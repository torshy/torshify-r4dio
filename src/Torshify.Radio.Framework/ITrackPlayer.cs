using System;

namespace Torshify.Radio.Framework
{
    public interface ITrackPlayer : IDisposable
    {
        #region Events

        event EventHandler IsPlayingChanged;

        event EventHandler IsBufferingChanged;

        event EventHandler<TrackBufferingEventArgs> BufferingProgressChanged;

        event EventHandler<TrackEventArgs> TrackComplete;

        event EventHandler<TrackProgressEventArgs> TrackProgress;

        #endregion Events

        #region Properties

        bool IsPlaying
        {
            get;
        }

        bool IsMuted
        {
            get; 
            set;
        }

        bool IsBuffering
        {
            get;
        }

        double Volume
        {
            get;
            set;
        }

        double BufferingProgress
        {
            get;
        }

        TimeSpan Position
        {
            get;
            set;
        }

        bool CanSeek
        {
            get;
        }

        #endregion Properties

        #region Methods

        bool CanPlay(Track track);

        void Load(Track track);

        void Stop();

        void Play();

        void Pause();

        void Initialize();

        #endregion Methods
    }
}