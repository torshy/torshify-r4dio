using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Services
{
    [Export(typeof(ITileService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TileService : ITileService
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly ObservableCollection<Tile> _tiles;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public TileService(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _tiles = new ObservableCollection<Tile>();
        }

        #endregion Constructors

        #region Events

        public event EventHandler<TileEventArgs> TileAdded;

        #endregion Events

        #region Properties

        public IEnumerable<Tile> Tiles
        {
            get
            {
                return _tiles;
            }
        }

        #endregion Properties

        #region Methods

        public void Add<T>(TileData tileData, params Tuple<string, string>[] parameters)
        {
            Add<T>(tileData, AppRegions.ViewRegion, parameters);
        }

        public void Add<T>(TileData tileData, string regionName, params Tuple<string, string>[] parameters)
        {
            if (_regionManager.Regions.ContainsRegionWithName(regionName))
            {
                IRegion viewRegion = _regionManager.Regions[regionName];

                if (!viewRegion.Views.Any(v => v.GetType() == typeof(T)))
                {
                    _regionManager.RegisterViewWithRegion(regionName, typeof(T));
                }
            }
            else
            {
                _regionManager.RegisterViewWithRegion(regionName, typeof(T));
            }

            UriQuery query = new UriQuery();

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    query.Add(parameter.Item1, parameter.Item2);
                }
            }

            Uri navigationUri = new Uri(typeof(T).FullName + query, UriKind.RelativeOrAbsolute);
            var item = new Tile(navigationUri, tileData, regionName);
            _tiles.Add(item);
            OnTileAdded(new TileEventArgs(item));
        }

        protected void OnTileAdded(TileEventArgs e)
        {
            var handler = TileAdded;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion Methods
    }
}