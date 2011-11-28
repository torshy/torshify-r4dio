using System;
using System.ComponentModel.Composition;
using System.Timers;

using EightTracks;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks
{
    [Export(typeof(IRadioTrackPlayer))]
    public class EightTracksRadioTrackPlayer : MediaPlayerRadioTrackPlayer
    {
        #region Fields

        private Timer _reportTimer;

        #endregion Fields

        #region Constructors

        public EightTracksRadioTrackPlayer()
        {
            _reportTimer = new Timer(30000);
            _reportTimer.Elapsed += ReportTimerOnElapsed;
        }

        #endregion Constructors

        #region Methods

        public override bool CanPlay(IRadioTrack radioTrack)
        {
            return radioTrack is EightTracksRadioTrack;
        }

        public override void Load(IRadioTrack track)
        {
            var eightTracksRadioTrack = track as EightTracksRadioTrack;

            if (eightTracksRadioTrack != null && !string.IsNullOrEmpty(eightTracksRadioTrack.Track.Url))
            {
                CurrentTrack = eightTracksRadioTrack;
                CurrentTrackElapsed = TimeSpan.Zero;

                Player.Open(eightTracksRadioTrack.Uri);
            }
        }

        public override void Play()
        {
            base.Play();
            _reportTimer.Start();
        }

        public override void Stop()
        {
            base.Stop();
            _reportTimer.Stop();
        }

        protected override void OnMediaEnded(object sender, EventArgs e)
        {
            base.OnMediaEnded(sender, e);

            _reportTimer.Stop();
        }

        private void ReportTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var track = CurrentTrack as EightTracksRadioTrack;

            if (track != null)
            {
                using (var session = new EightTracksSession(EightTracksRadioStation.ApiKey))
                {
                    session.Query<Play>().Report(track.TokenID, track.Track.ID, track.MixID);
                }
            }
        }

        #endregion Methods
    }
}