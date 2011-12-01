using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using Grooveshark_Sharp;
using Grooveshark_Sharp.TinySong;

using Microsoft.Practices.Prism.Logging;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Grooveshark
{
    [RadioTrackSourceMetadata(Name = "Grooveshark")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GroovesharkRadioTrackSource : IRadioTrackSource
    {
        #region Fields

        public static GroovesharkSession Session;

        // Note  : Yes yes i know, but the damn thing is getting rate limit exceeded all the time when some stupid bug occurrs. 
        // This way i can rotate between two keys when one is wrecked :x Sowwy grooveshark.
        private const string TinySongApiKey = "d55e33ec381ba3566d3984af64f65d8c";
        private const string TinySongApiKey1 = "e5496c78cf106ccdc54aac2a835ec838";
        private readonly ILoggerFacade _logger;
        private bool _rateLimitExceeded;
        private DateTime _rateLimitExceededTimestamp;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public GroovesharkRadioTrackSource(ILoggerFacade logger)
        {
            _logger = logger;
        }

        #endregion Constructors

        #region Methods

        public IEnumerable<RadioTrack> GetTracksByAlbum(string artist, string album)
        {
            List<RadioTrack> tracks = new List<RadioTrack>();

            if (IsRateLimitExceeded())
            {
                try
                {
                    GroovesharkSearch search = new GroovesharkSearch(Session);
                    var searchResult = search.Search(artist + " " +  album);

                    if (searchResult != null)
                    {
                        foreach (var s in searchResult)
                        {
                            if (artist.Equals(s.ArtistName, StringComparison.InvariantCultureIgnoreCase) &&
                                album.Equals(s.AlbumName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                tracks.Add(new GroovesharkRadioTrack
                                               {
                                                   Name = s.SongName,
                                                   Album = s.AlbumName,
                                                   Artist = s.ArtistName,
                                                   SongID = s.SongId,
                                                   AlbumID = s.AlbumId,
                                                   ArtistID = s.ArtistId,
                                                   AlbumArt =
                                                       "http://images.grooveshark.com/static/albums/90_" + s.AlbumId +
                                                       ".jpg"
                                               });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Log("Grooveshark - " + e, Category.Info, Priority.Medium);
                }
            }
            else
            {
                try
                {
                    TinySongSession session = new TinySongSession(TinySongApiKey);
                    var result = session.Search(artist, 32);

                    if (result != null)
                    {
                        _rateLimitExceeded = false;
                        foreach (var s in result)
                        {
                            if (artist.Equals(s.ArtistName, StringComparison.InvariantCultureIgnoreCase) &&
                                album.Equals(s.AlbumName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                tracks.Add(new GroovesharkRadioTrack
                                               {
                                                   Name = s.SongName,
                                                   Album = s.AlbumName,
                                                   Artist = s.ArtistName,
                                                   SongID = s.SongId,
                                                   AlbumID = s.AlbumId,
                                                   ArtistID = s.ArtistId,
                                                   AlbumArt =
                                                       "http://images.grooveshark.com/static/albums/90_" + s.AlbumId +
                                                       ".jpg"
                                               });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _rateLimitExceededTimestamp = DateTime.Now;
                    _rateLimitExceeded = true;
                    _logger.Log("Grooveshark - " + e, Category.Info, Priority.Medium);
                }

            }

            return tracks;
        }

        public IEnumerable<RadioTrack> GetTracksByArtist(string artist, int offset, int count)
        {
            List<RadioTrack> tracks = new List<RadioTrack>();

            if (IsRateLimitExceeded())
            {
                try
                {
                    GroovesharkSearch search = new GroovesharkSearch(Session);
                    var searchResult = search.Search(artist);

                    if (searchResult != null)
                    {
                        foreach (var s in searchResult.Skip(offset).Take(count))
                        {
                            if (artist.Equals(s.ArtistName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                tracks.Add(new GroovesharkRadioTrack
                                {
                                    Name = s.SongName,
                                    Album = s.AlbumName,
                                    Artist = s.ArtistName,
                                    SongID = s.SongId,
                                    AlbumID = s.AlbumId,
                                    ArtistID = s.ArtistId,
                                    AlbumArt =
                                        "http://images.grooveshark.com/static/albums/90_" + s.AlbumId +
                                        ".jpg"
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Log("Grooveshark - " + e, Category.Info, Priority.Medium);
                }
            }
            else
            {
                try
                {
                    TinySongSession session = new TinySongSession(TinySongApiKey);
                    var result = session.Search(artist, count);

                    if (result != null)
                    {
                        _rateLimitExceeded = false;
                        foreach (var s in result)
                        {
                            if (artist.Equals(s.ArtistName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                tracks.Add(new GroovesharkRadioTrack
                                               {
                                                   Name = s.SongName,
                                                   Album = s.AlbumName,
                                                   Artist = s.ArtistName,
                                                   SongID = s.SongId,
                                                   AlbumID = s.AlbumId,
                                                   ArtistID = s.ArtistId,
                                                   AlbumArt =
                                                       "http://images.grooveshark.com/static/albums/90_" + s.AlbumId +
                                                       ".jpg"
                                               });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _rateLimitExceededTimestamp = DateTime.Now;
                    _rateLimitExceeded = true;
                    _logger.Log("Grooveshark - " + e.Message, Category.Info, Priority.Medium);
                }
            }
            return tracks;
        }

        public IEnumerable<RadioTrack> GetTracksByName(string name, int offset, int count)
        {
            List<RadioTrack> tracks = new List<RadioTrack>();

            if (IsRateLimitExceeded())
            {
                try
                {
                    GroovesharkSearch search = new GroovesharkSearch(Session);
                    var searchResult = search.Search(name);

                    if (searchResult != null)
                    {
                        foreach (var s in searchResult.Skip(offset).Take(count))
                        {
                            tracks.Add(new GroovesharkRadioTrack
                            {
                                Name = s.SongName,
                                Album = s.AlbumName,
                                Artist = s.ArtistName,
                                SongID = s.SongId,
                                AlbumID = s.AlbumId,
                                ArtistID = s.ArtistId,
                                AlbumArt =
                                    "http://images.grooveshark.com/static/albums/90_" + s.AlbumId +
                                    ".jpg"
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Log("Grooveshark - " + e, Category.Info, Priority.Medium);
                }
            }
            else
            {
                try
                {
                    TinySongSession session = new TinySongSession(TinySongApiKey);
                    var result = session.Search(name, count);

                    if (result != null)
                    {
                        _rateLimitExceeded = false;
                        return result.Select(s =>
                                             new GroovesharkRadioTrack
                                                 {
                                                     Name = s.SongName,
                                                     Album = s.AlbumName,
                                                     Artist = s.ArtistName,
                                                     SongID = s.SongId,
                                                     AlbumID = s.AlbumId,
                                                     ArtistID = s.ArtistId,
                                                     AlbumArt =
                                                         "http://images.grooveshark.com/static/albums/90_" + s.AlbumId +
                                                         ".jpg"
                                                 });
                    }
                }
                catch (Exception e)
                {
                    _rateLimitExceededTimestamp = DateTime.Now;
                    _rateLimitExceeded = true;
                    _logger.Log("Grooveshark - " + e.Message, Category.Info, Priority.Medium);
                }
            }

            return tracks;
        }

        private bool IsRateLimitExceeded()
        {
            return _rateLimitExceeded && DateTime.Now - _rateLimitExceededTimestamp < TimeSpan.FromMinutes(10);
        }

        public void Initialize()
        {
            Task.Factory.StartNew(() =>
                                      {
                                          Session = new GroovesharkSession();
                                          Session.Connect();
                                      });
        }

        #endregion Methods
    }
}