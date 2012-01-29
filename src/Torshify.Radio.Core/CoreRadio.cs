using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core
{
    [Export(typeof(IRadio))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CoreRadio : IRadio
    {
        #region Fields

        private readonly ConcurrentQueue<ITrackStream> _trackStreamQueue;

        private CorePlayer _corePlayer;
        private readonly ILoadingIndicatorService _loadingIndicatorService;
        private ITrackStream _currentTrackStream;
        private ConcurrentQueue<Track> _trackQueue;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public CoreRadio(CorePlayer corePlayer, ILoadingIndicatorService loadingIndicatorService)
        {
            _trackStreamQueue = new ConcurrentQueue<ITrackStream>();
            _trackQueue = new ConcurrentQueue<Track>();
            _corePlayer = corePlayer;
            _loadingIndicatorService = loadingIndicatorService;
            _corePlayer.Volume = 0.2;
            _corePlayer.TrackComplete += OnTrackComplete;
            _corePlayer.Initialize();
        }

        #endregion Constructors

        #region Properties

        public Track CurrentTrack
        {
            get;
            private set;
        }

        public Track UpcomingTrack
        {
            get;
            private set;
        }

        public IEnumerable<ITrackStream> TrackStreams
        {
            get
            {
                return _trackStreamQueue;
            }
        }

        [ImportMany]
        public IEnumerable<Lazy<ITrackPlayer, ITrackPlayerMetadata>> TrackPlayers
        {
            get;
            set;
        }

        [ImportMany]
        public IEnumerable<Lazy<ITrackSource, ITrackSourceMetadata>> TrackSources
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public Track FromLink(TrackLink trackLink)
        {
            var source = TrackSources.FirstOrDefault(s => s.Value.SupportsLink(trackLink));

            if (source != null)
            {
                return source.Value.FromLink(trackLink);
            }

            return null;
        }

        public IEnumerable<TrackContainer> GetAlbumsByArtist(string artist)
        {
            List<TrackContainer> albums = new List<TrackContainer>();

            foreach (var source in TrackSources)
            {
                albums.AddRange(source.Value.GetAlbumsByArtist(artist));
            }

            return albums;
        }

        public IEnumerable<Track> GetTracksByName(string name)
        {
            List<Track> tracks = new List<Track>();

            foreach (var source in TrackSources)
            {
                tracks.AddRange(source.Value.GetTracksByName(name));
            }

            return tracks;
        }

        public void PlayTrackStream(ITrackStream trackStream)
        {
            Task.Factory.StartNew(() =>
                                  {
                                      _loadingIndicatorService.Push();
                                      _currentTrackStream = trackStream;
                                      _trackQueue = new ConcurrentQueue<Track>();
                                      GetNextBatch();
                                      MoveToNextTrack();
                                      PeekToNextTrack();
                                      _loadingIndicatorService.Pop();
                                  });
        }

        public void QueueTrackStream(ITrackStream trackStream)
        {
            _trackStreamQueue.Enqueue(trackStream);
        }

        public bool SupportsLink(TrackLink trackLink)
        {
            return TrackSources.Any(s => s.Value.SupportsLink(trackLink));
        }

        private void GetNextBatch()
        {
            if (_currentTrackStream.MoveNext())
            {
                var tracks = _currentTrackStream.Current;

                foreach (var track in tracks)
                {
                    _trackQueue.Enqueue(track);
                }
            }
        }

        private void PeekToNextTrack()
        {
            Track upcomingTrack;
            if (_trackQueue.TryPeek(out upcomingTrack))
            {
                UpcomingTrack = upcomingTrack;
            }
            else
            {
                UpcomingTrack = null;
            }
        }

        private void MoveToNextTrack()
        {
            Track firstTrack;
            if (_trackQueue.TryDequeue(out firstTrack))
            {
                _corePlayer.Load(firstTrack);
                _corePlayer.Play();

                CurrentTrack = firstTrack;
            }
            else
            {
                CurrentTrack = null;
            }
        }

        private void OnTrackComplete(object sender, TrackEventArgs e)
        {
            if (_trackQueue.IsEmpty)
            {
                GetNextBatch();
            }

            MoveToNextTrack();
            PeekToNextTrack();
        }

        #endregion Methods
    }
}