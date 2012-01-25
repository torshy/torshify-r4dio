using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Events;
using Torshify.Radio.Core.Views.NowPlaying;
using Torshify.Radio.Framework;
using System.Linq;

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

        [Import]
        public IBackdropService BackdropService
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
            BackdropService
                .Query("Ed Sheeran")
                .ContinueWith(t =>
                              {
                                  if (t.Result.Any())
                                  {
                                      _tileData.BackgroundImage = new Uri(t.Result.FirstOrDefault(), UriKind.RelativeOrAbsolute);
                                  }
                              }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}