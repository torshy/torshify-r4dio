using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Events;
using Torshify.Radio.Core.Views.NowPlaying;
using Torshify.Radio.Framework;
using System.Linq;
using WpfShaderEffects;

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
            _tileData.BackgroundImage = null;

            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            TileService.Add<NowPlayingView>(_tileData, AppRegions.MainRegion);
            BackdropService
                .Query("bad religion")
                .ContinueWith(t =>
                {
                    if (t.Result.Any())
                    {
                        if (t.Result.Count() > 1)
                        {
                            //_tileData.BackgroundImage = new Uri(t.Result.FirstOrDefault(), UriKind.RelativeOrAbsolute);
                            //_tileData.Effect = new ColorToneShaderEffect
                            //                    {
                            //                        DarkColor = Colors.Black,
                            //                        LightColor = Colors.DarkGray,
                            //                        Desaturation = 0.5,
                            //                        Toned = 1.0
                            //                    };

                            // TODO : Freeze as much as possible
                            BitmapImage first = new BitmapImage(new Uri(t.Result.First(), UriKind.RelativeOrAbsolute));
                            BitmapImage second = new BitmapImage(new Uri(t.Result.Skip(1).First(), UriKind.RelativeOrAbsolute));
                            first.Freeze();
                            second.Freeze();

                            var firstInput = new ImageBrush(first) { Stretch = Stretch.Fill };
                            var secondInput = new ImageBrush(second) { Stretch = Stretch.Fill };
                            firstInput.Freeze();
                            secondInput.Freeze();

                            _tileData.Effect = new FadeTransitionShaderEffect
                                               {
                                                   Input = firstInput,
                                                   SecondInput = secondInput,
                                               };

                            DoubleAnimation da = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(5)));
                            da.AccelerationRatio = 0.5;
                            da.DecelerationRatio = 0.5;
                            da.RepeatBehavior = RepeatBehavior.Forever;
                            da.AutoReverse = true;

                            _tileData.Effect.BeginAnimation(FadeTransitionShaderEffect.ProgressProperty, da);
                        }
                        else
                        {
                            _tileData.BackgroundImage = new Uri(t.Result.FirstOrDefault(), UriKind.RelativeOrAbsolute);
                        }
                    }
                }, ui);
        }
    }
}