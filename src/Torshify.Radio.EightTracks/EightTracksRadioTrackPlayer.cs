using System;
using System.Timers;
using System.Windows;
using EightTracks;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks
{
    [RadioTrackPlayerMetadata(Name = "8tracks", Icon = "pack://siteoforigin:,,,/Resources/Icons/EightTracks_Logo.jpg")]
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

        public override bool CanPlay(RadioTrack radioTrack)
        {
            return radioTrack is EightTracksRadioTrack;
        }

        public override void Load(RadioTrack track)
        {
            var eightTracksRadioTrack = track as EightTracksRadioTrack;

            if (eightTracksRadioTrack != null && !string.IsNullOrEmpty(eightTracksRadioTrack.Track.Url))
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                                                                     {
                                                                         CurrentTrack = eightTracksRadioTrack;
                                                                         CurrentTrackElapsed = TimeSpan.Zero;

                                                                         Player.Open(eightTracksRadioTrack.Uri);
                                                                     }));
            }
        }

        public override void Play()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                                                                 {
                                                                     base.Play();
                                                                     _reportTimer.Start();
                                                                 }));
        }

        public override void Stop()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                                                                 {
                                                                     base.Stop();
                                                                     _reportTimer.Stop();
                                                                 }));
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