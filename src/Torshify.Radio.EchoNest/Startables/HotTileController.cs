using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using EchoNest;
using EchoNest.Artist;

using Hardcodet.Scheduling;

using Microsoft.Practices.Prism.Logging;

using Torshify.Radio.Framework;

using WpfShaderEffects;

namespace Torshify.Radio.EchoNest.Startables
{
    public class HotTileController : IStartable
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private readonly ILoggerFacade _logger;
        private readonly Random _random;
        private readonly Uri _tileIcon = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MS_hot.png");
        private readonly ITileService _tileService;

        private Scheduler _scheduler;
        private TileData _tileData;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public HotTileController(
            Dispatcher dispatcher,
            Scheduler scheduler,
            ITileService tileService,
            ILoggerFacade logger)
        {
            _random = new Random();
            _dispatcher = dispatcher;
            _scheduler = scheduler;
            _tileService = tileService;
            _logger = logger;
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            _tileData = new TileData
            {
                Title = "Hot artists",
                BackgroundImage = _tileIcon
            };

            _dispatcher.BeginInvoke(new Action(() => _tileService.Add<Views.Hot.HotArtistsView>(_tileData)), DispatcherPriority.ContextIdle);

            Job<TileData> job = new Job<TileData>("HotTile");
            job.Data = _tileData;
            job.Run.From(DateTime.Now).Every.Seconds(30);
            _scheduler.SubmitJob(job, HotTileJobExecution);
        }

        private void HotTileJobExecution(Job<TileData> job, TileData tile)
        {
            try
            {
                var imageBackground = _random.NextDouble();

                if (imageBackground > 0.2)
                {
                    using (var session = new EchoNestSession(EchoNestModule.ApiKey))
                    {
                        var response = session.Query<TopHottt>().Execute(bucket: ArtistBucket.Images);

                        if (response != null && response.Status.Code == ResponseCode.Success)
                        {
                            var index = _random.Next(0, response.Artists.Count - 1);
                            var artist = response.Artists[index];

                            if (artist.Images.Count > 0)
                            {
                                tile.Effect = new ColorToneShaderEffect
                                {
                                    DarkColor = Colors.Black,
                                    LightColor = Colors.DarkGray,
                                    Desaturation = 0.2,
                                    Toned = 1.0
                                };

                                tile.Effect.Freeze();

                                index = _random.Next(0, artist.Images.Count - 1);
                                tile.BackgroundImage = new Uri(artist.Images[index].Url, UriKind.RelativeOrAbsolute);

                                _dispatcher.BeginInvoke(new Action(() =>
                                {
                                    tile.Content = new TextBlock
                                    {
                                        Text = artist.Name,
                                        TextTrimming = TextTrimming.CharacterEllipsis,
                                        Margin = new Thickness(4),
                                        HorizontalAlignment = HorizontalAlignment.Left,
                                        VerticalAlignment = VerticalAlignment.Top
                                    };
                                }));
                            }
                        }
                    }
                }
                else
                {
                    tile.Effect = null;
                    tile.BackgroundImage = _tileIcon;
                    tile.Content = null;
                }
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), Category.Exception, Priority.Low);
            }
        }

        #endregion Methods
    }
}