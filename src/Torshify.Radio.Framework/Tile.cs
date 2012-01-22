using System;

namespace Torshify.Radio.Framework
{
    public class Tile
    {
        #region Constructors

        public Tile(Uri navigationUri, TileData data)
        {
            NavigationUri = navigationUri;
            Data = data;
        }

        #endregion Constructors

        #region Properties

        public TileData Data
        {
            get; private set;
        }

        public Uri NavigationUri
        {
            get; private set;
        }

        #endregion Properties
    }
}