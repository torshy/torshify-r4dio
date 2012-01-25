using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Events;
using Torshify.Radio.Core.Views.NowPlaying;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Startables
{
    public class NowPlayingTile : IStartable
    {
        private TileData _tileData;

        [Import]
        public ITileService TileService
        {
            get; 
            set;
        }

        [Import]
        public IEventAggregator EventAggregator
        {
            get; 
            set;
        }

        public void Start()
        {
            _tileData = new TileData();
            _tileData.Title = "Now playing";
            _tileData.IsLarge = true;

            TileService.Add<NowPlayingView>(_tileData, AppRegions.MainRegion);
        }
    }
}