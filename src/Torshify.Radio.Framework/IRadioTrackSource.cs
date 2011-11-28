using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface IRadioTrackSource
    {
        #region Methods

        IEnumerable<IRadioTrack> GetTracksByAlbum(string artist, string album);

        IEnumerable<IRadioTrack> GetTracksByArtist(string artist, int offset, int count);

        IEnumerable<IRadioTrack> GetTracksByName(string name, int offset, int count);

        void Initialize();

        #endregion Methods
    }
}