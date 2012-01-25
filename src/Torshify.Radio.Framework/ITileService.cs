using System;
using System.Collections.Generic;

namespace Torshify.Radio.Framework
{
    public interface ITileService
    {
        #region Properties

        IEnumerable<Tile> Tiles
        {
            get;
        }

        #endregion Properties

        #region Methods

        void Add<T>(TileData tileData, params Tuple<string, string>[] parameters);

        void Add<T>(TileData tileData, string regionName, params Tuple<string, string>[] parameters);

        #endregion Methods
    }
}