using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Services
{
    [Export(typeof(ITileService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TileService : NotificationObject, ITileService
    {
        #region Fields

        private readonly ObservableCollection<string> _observedRegions;
        private readonly IRegionManager _regionManager;
        private readonly ObservableCollection<Tile> _tiles;

        private Tile _currentTile;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public TileService(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _tiles = new ObservableCollection<Tile>();
            _observedRegions = new ObservableCollection<string>();
            _observedRegions.CollectionChanged += ObservedRegionsChanged;
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

        public Tile CurrentTile
        {
            get
            {
                return _currentTile;
            }
            private set
            {
                if (_currentTile != value)
                {
                    _currentTile = value;
                    RaisePropertyChanged("CurrentTile");
                }
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

            if (!_observedRegions.Contains(regionName))
            {
                _observedRegions.Add(regionName);
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

        protected void ObservedRegionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                string regionName = e.NewItems[0].ToString();

                if (_regionManager.Regions.ContainsRegionWithName(regionName))
                {
                    var region = _regionManager.Regions[regionName];
                    region.NavigationService.Navigated += ObservedRegionNavigated;
                }
            }
        }

        protected void ObservedRegionNavigated(object sender, RegionNavigationEventArgs e)
        {
            CurrentTile = _tiles.FirstOrDefault(t => t.NavigationUri.OriginalString.StartsWith(e.Uri.OriginalString));
        }

        #endregion Methods
    }
}