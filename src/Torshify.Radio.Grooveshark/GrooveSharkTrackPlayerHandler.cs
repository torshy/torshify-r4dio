using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Timers;

using Microsoft.Practices.Prism.Logging;

using NAudio.Wave;
using Torshify.Radio.Framework;
using Torshify.Radio.Grooveshark.NAudio;

using Timer = System.Timers.Timer;

namespace Torshify.Radio.Grooveshark
{
    public class GrooveSharkTrackPlayerHandler
    {
        #region Fields

        private readonly Action<bool> _isBuffering;
        private readonly Action<bool> _isPlaying;
        private readonly ILoggerFacade _log;
        private readonly GroovesharkTrack _track;
        private readonly Action<Track> _trackComplete;
        private readonly Action<double, double> _trackProgress;

        private BufferedWaveProvider _bufferedWaveProvider;
        private Thread _bufferThread;
        private TimeSpan _elapsedTimeSpan;
        private bool _fullyDownloaded;
        private StreamingPlaybackState _playbackState;
        private Timer _timer;
        private float _volume;
        private VolumeWaveProvider16 _volumeProvider;
        private WaveOut _waveOut;

        #endregion Fields

        #region Constructors

        public GrooveSharkTrackPlayerHandler(
            ILoggerFacade log,
            GroovesharkTrack track,
            Action<Track> trackComplete,
            Action<bool> isPlaying,
            Action<bool> isBuffering,
            Action<double, double> trackProgress)
        {
            _log = log;
            _track = track;
            _trackComplete = trackComplete;
            _isPlaying = isPlaying;
            _isBuffering = isBuffering;
            _trackProgress = trackProgress;

            Volume = 0.5f;
        }

        #endregion Constructors

        #region Enumerations

        enum StreamingPlaybackState
        {
            Stopped,
            Playing,
            Buffering,
            Paused
        }

        #endregion Enumerations

        #region Properties

        public float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;

                if (_volumeProvider != null)
                {
                    _volumeProvider.Volume = value;
                }
            }
        }

        #endregion Properties

        #region Methods

        public void Initialize(Stream stream)
        {
            _timer = new Timer(250);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();

            _bufferThread = new Thread(StreamMp3);
            _bufferThread.IsBackground = true;
            _bufferThread.Start(stream);
        }

        public void Pause()
        {
            if (_waveOut != null)
            {
                _waveOut.Pause();
                _isPlaying(false);
                _playbackState = StreamingPlaybackState.Paused;
            }
        }

        public void Play()
        {
            if (_playbackState == StreamingPlaybackState.Paused)
            {
                if (_waveOut != null)
                {
                    _waveOut.Play();
                    _isPlaying(true);
                    _playbackState = StreamingPlaybackState.Playing;
                }

                _timer.Enabled = true;
            }
        }

        public void Stop()
        {
            try
            {
                _timer.Enabled = false;
                _timer.Dispose();
                _playbackState = StreamingPlaybackState.Stopped;

                if (_waveOut != null)
                {
                    _waveOut.Stop();
                    _waveOut.Dispose();
                    _waveOut = null;
                }

                if (_bufferThread != null && _bufferThread.IsAlive)
                {
                    _bufferThread.Join(1000);
                }
            }
            catch (Exception e)
            {
                _log.Log("Grooveshark: Error while stopping player. " + e.Message, Category.Info, Priority.Medium);
            }
            finally
            {
                _isPlaying(false);
                _elapsedTimeSpan = TimeSpan.Zero;
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_playbackState != StreamingPlaybackState.Stopped)
            {
                if (_waveOut == null && _bufferedWaveProvider != null)
                {
                    _log.Log("Grooveshark: Initializing", Category.Info, Priority.Medium);

                    try
                    {
                        _waveOut = new WaveOut();
                        _volumeProvider = new VolumeWaveProvider16(_bufferedWaveProvider);
                        _volumeProvider.Volume = Volume;
                        _waveOut.Init(_volumeProvider);
                    }
                    catch (Exception ex)
                    {
                        _log.Log("Grooveshark: " + ex.ToString(), Category.Exception, Priority.High);
                        _elapsedTimeSpan = TimeSpan.Zero;
                        _playbackState = StreamingPlaybackState.Stopped;
                        _timer.Stop();
                        _isPlaying(false);
                        _trackComplete(_track);
                    }
                }
                else if (_bufferedWaveProvider != null)
                {
                    var bufferedSeconds = _bufferedWaveProvider.BufferedDuration.TotalSeconds;
                    // make it stutter less if we buffer up a decent amount before playing
                    if (bufferedSeconds < 0.5 && _playbackState == StreamingPlaybackState.Playing && !_fullyDownloaded)
                    {
                        _isBuffering(true);

                        _log.Log("Grooveshark: Buffering..", Category.Info, Priority.Medium);

                        _playbackState = StreamingPlaybackState.Buffering;

                        if (_waveOut != null)
                        {
                            _waveOut.Pause();
                            _isPlaying(false);
                        }
                    }
                    else if (bufferedSeconds > 4 && _playbackState == StreamingPlaybackState.Buffering)
                    {
                        _log.Log("Grooveshark: Buffering complete", Category.Info, Priority.Medium);

                        if (_waveOut != null)
                        {
                            _waveOut.Play();
                            _playbackState = StreamingPlaybackState.Playing;
                            _isPlaying(true);
                        }

                        _isBuffering(false);
                    }
                    else if (_fullyDownloaded && bufferedSeconds < 0.5)
                    {
                        _log.Log("Grooveshark: Buffer empty and the stream is fully downloaded. Complete..", Category.Info, Priority.Medium);
                        _elapsedTimeSpan = TimeSpan.Zero;
                        _isPlaying(false);
                        _playbackState = StreamingPlaybackState.Stopped;
                        _timer.Stop();
                        _trackComplete(_track);
                    }
                }

                if (_playbackState == StreamingPlaybackState.Playing)
                {
                    _elapsedTimeSpan = _elapsedTimeSpan.Add(TimeSpan.FromMilliseconds(_timer.Interval));
                    _trackProgress(_track.TotalDuration.TotalMilliseconds, _elapsedTimeSpan.TotalMilliseconds);
                }
            }
        }

        private void StreamMp3(object state)
        {
            Stream responseStream = (Stream)state;
            byte[] buffer = new byte[16384 * 4]; // needs to be big enough to hold a decompressed frame

            IMp3FrameDecompressor decompressor = null;
            try
            {
                using (responseStream)
                {
                    _playbackState = StreamingPlaybackState.Buffering;

                    using (var readFullyStream = new ReadFullyStream(responseStream))
                    {
                        do
                        {
                            if (_bufferedWaveProvider != null &&
                                _bufferedWaveProvider.BufferLength - _bufferedWaveProvider.BufferedBytes <
                                _bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 4)
                            {
                                Thread.Sleep(500);
                            }
                            else
                            {
                                Mp3Frame frame;

                                try
                                {
                                    frame = Mp3Frame.LoadFromStream(readFullyStream);

                                    if (frame == null)
                                    {
                                        _fullyDownloaded = true;
                                        break;
                                    }
                                }
                                catch (EndOfStreamException e)
                                {
                                    _log.Log(e.Message, Category.Warn, Priority.Medium);
                                    _fullyDownloaded = true;
                                    // reached the end of the MP3 file / stream
                                    break;
                                }
                                catch (WebException e)
                                {
                                    _log.Log(e.Message, Category.Warn, Priority.Medium);
                                    // probably we have aborted download from the GUI thread
                                    break;
                                }
                                catch (Exception e)
                                {
                                    _log.Log(e.Message, Category.Exception, Priority.High);
                                    break;
                                }

                                if (decompressor == null)
                                {
                                    WaveFormat waveFormat = new Mp3WaveFormat(44100,
                                                                              frame.ChannelMode == ChannelMode.Mono
                                                                                  ? 1
                                                                                  : 2, frame.FrameLength, frame.BitRate);
                                    decompressor = new AcmMp3FrameDecompressor(waveFormat);
                                    _bufferedWaveProvider = new BufferedWaveProvider(decompressor.OutputFormat);

                                    if (_track.TotalDuration != TimeSpan.Zero)
                                    {
                                        _bufferedWaveProvider.BufferDuration = _track.TotalDuration;
                                    }
                                    else
                                    {
                                        _bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(10);
                                    }
                                }

                                if (_bufferedWaveProvider != null)
                                {
                                    try
                                    {
                                        int decompressed = decompressor.DecompressFrame(frame, buffer, 0);
                                        _bufferedWaveProvider.AddSamples(buffer, 0, decompressed);
                                    }
                                    catch (Exception e)
                                    {
                                        _fullyDownloaded = true;
                                        _log.Log("Grooveshark: Error decompressing frame: " + e.Message, Category.Exception, Priority.Medium);
                                        break;
                                    }
                                }
                            }

                        } while (_playbackState != StreamingPlaybackState.Stopped);

                        // was doing this in a finally block, but for some reason
                        // we are hanging on response stream .Dispose so never get there
                        if (decompressor != null)
                        {
                            decompressor.Dispose();
                            decompressor = null;
                        }
                    }
                }
            }
            finally
            {
                if (decompressor != null)
                {
                    decompressor.Dispose();
                    decompressor = null;
                }
            }

            _log.Log("Grooveshark: Buffer thread exiting", Category.Info, Priority.Medium);
        }

        #endregion Methods
    }
}