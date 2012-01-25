using System;

namespace Torshify.Radio.Framework
{
    public class Tile
    {
        #region Constructors

        public Tile(Uri navigationUri, TileData data, string targetRegionName)
        {
            NavigationUri = navigationUri;
            Data = data;
            TargetRegionName = targetRegionName;
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

        public string TargetRegionName
        {
            get; private set;
        }

        #endregion Properties
    }
}