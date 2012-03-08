using System;

namespace Torshify.Radio.Framework
{
    public class TileEventArgs : EventArgs
    {
        #region Constructors

        public TileEventArgs(Tile tile)
        {
            Tile = tile;
        }

        #endregion Constructors

        #region Properties

        public Tile Tile
        {
            get; private set;
        }

        #endregion Properties
    }
}