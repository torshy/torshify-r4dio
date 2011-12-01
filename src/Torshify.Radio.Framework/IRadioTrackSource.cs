using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface IRadioTrackSource
    {
        #region Methods

        IEnumerable<RadioTrack> GetTracksByAlbum(string artist, string album);

        IEnumerable<RadioTrack> GetTracksByArtist(string artist, int offset, int count);

        IEnumerable<RadioTrack> GetTracksByName(string name, int offset, int count);

        void Initialize();

        #endregion Methods
    }
}