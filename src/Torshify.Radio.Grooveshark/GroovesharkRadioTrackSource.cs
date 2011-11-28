using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using Grooveshark_Sharp;
using Grooveshark_Sharp.TinySong;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Grooveshark
{
    [Export(typeof(IRadioTrackSource))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GroovesharkRadioTrackSource : IRadioTrackSource
    {
        #region Fields

        public static GroovesharkSession Session;

        private const string TinySongApiKey = "d55e33ec381ba3566d3984af64f65d8c";

        #endregion Fields

        #region Methods

        public IEnumerable<IRadioTrack> GetTracksByAlbum(string artist, string album)
        {
            return new IRadioTrack[0];
        }

        public IEnumerable<IRadioTrack> GetTracksByArtist(string artist, int offset, int count)
        {
            try
            {
                TinySongSession session = new TinySongSession(TinySongApiKey);
                var result = session.Search(artist, 1);

                if (result != null)
                {
                    return result.Select(s =>
                                         new GroovesharkRadioTrack
                                             {
                                                 Name = s.SongName,
                                                 Album = s.AlbumName,
                                                 Artist = s.ArtistName,
                                                 SongID = s.SongId
                                             });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new IRadioTrack[0];
        }

        public IEnumerable<IRadioTrack> GetTracksByName(string name, int offset, int count)
        {
            try
            {
                TinySongSession session = new TinySongSession(TinySongApiKey);
                var result = session.Search(name, count);

                if (result != null)
                {
                    return result.Select(s =>
                                         new GroovesharkRadioTrack
                                         {
                                             Name = s.SongName,
                                             Album = s.AlbumName,
                                             Artist = s.ArtistName,
                                             SongID = s.SongId
                                         });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new IRadioTrack[0];
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