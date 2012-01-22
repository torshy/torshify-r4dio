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

        public void Create<T>(TileData tileData, params Tuple<string, string>[] parameters)
            where T : IRadioStation
        {
            IRegion viewRegion = _regionManager.Regions[AppRegions.ViewRegion];

            if (!viewRegion.Views.Any(v => v.GetType() == typeof(T)))
            {
                _regionManager.RegisterViewWithRegion(AppRegions.ViewRegion, typeof(T));
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
            _tiles.Add(new Tile(navigationUri, tileData));
        }

        #endregion Methods
    }
}