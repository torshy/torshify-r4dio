using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Torshify.Radio.Core.Views.NowPlaying.UI
{
    public partial class ImageMapFrame : UserControl
    {
        #region Fields

        public const int SquareSize = 70;

        public static readonly int CanvasHeight = (int)SystemParameters.FullPrimaryScreenHeight + SquareSize;
        public static readonly int CanvasWidth = (int)SystemParameters.FullPrimaryScreenWidth + SquareSize;
        public static readonly DependencyProperty ImagesLocationProperty =
            DependencyProperty.Register("ImagesLocation", typeof(string), typeof(ImageMapFrame),
                new FrameworkPropertyMetadata((string)null));
        public static readonly DependencyProperty InitializeMapProperty =
            DependencyProperty.Register("InitializeMap", typeof(bool), typeof(ImageMapFrame),
                new FrameworkPropertyMetadata(false,
                    OnInitializeMapChanged));

        public readonly int Columns = CanvasWidth / SquareSize;
        public readonly int Rows = CanvasHeight / SquareSize;

        private readonly Random _random = new Random();
        private readonly double[] _sizeDistribution = new[] { 0.7, 0.1, 0.1, 0.05 };
        private readonly int[] _sizeElements = new[] { 1, 2, 3, 4 };

        #endregion Fields

        #region Constructors

        public ImageMapFrame()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public bool InitializeMap
        {
            get
            {
                return (bool)GetValue(InitializeMapProperty);
            }
            set
            {
                SetValue(InitializeMapProperty, value);
            }
        }

        public string ImagesLocation
        {
            get
            {
                return (string)GetValue(ImagesLocationProperty);
            }
            set
            {
                SetValue(ImagesLocationProperty, value);
            }
        }

        public Canvas MapCanvas
        {
            get
            {
                return _canvas;
            }
        }

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(ImagesLocation) && Directory.Exists(ImagesLocation))
            {
                Task<string[]>.Factory
                    .StartNew(FindAllImageFiles, ImagesLocation)
                    .ContinueWith(t => CreateMapData(t.Result))
                    .ContinueWith(t => CreateMap(t.Result))
                    .ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {
                            Console.WriteLine(t.Exception.ToString());
                        }
                    });
            }
        }

        protected virtual void OnInitializeMapChanged(bool oldInitializeMap, bool newInitializeMap)
        {
            if (newInitializeMap)
            {
                Initialize();
            }
            else
            {
                _canvas.Children.Clear();
            }
        }

        private static void OnInitializeMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageMapFrame target = (ImageMapFrame)d;
            bool oldInitializeMap = (bool)e.OldValue;
            bool newInitializeMap = target.InitializeMap;
            target.OnInitializeMapChanged(oldInitializeMap, newInitializeMap);
        }

        private string[] FindAllImageFiles(object state)
        {
            string location = state.ToString();
            return Directory.GetFiles(location, "*.jpg", SearchOption.AllDirectories);
        }

        private IEnumerable<ImageMapEntry> CreateMapData(string[] imageFiles)
        {
            var map = new Dictionary<KeyValuePair<int, int>, ImageMapEntry>();

            Func<int, int, int, bool> canFitBlockInAt = (row, column, blockSize) =>
                                                             {
                                                                 for (int r = row; r < row + blockSize; r++)
                                                                 {
                                                                     for (int c = column; c < column + blockSize; c++)
                                                                     {
                                                                         if (map.ContainsKey(new KeyValuePair<int, int>(r, c)))
                                                                             return false;
                                                                     }
                                                                 }

                                                                 return true;
                                                             };

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; )
                {
                    int maxBlockSize = _sizeElements.Length;
                    int blockSize = GetBlockSize(maxBlockSize);

                    while (!canFitBlockInAt(row, column, blockSize))
                    {
                        maxBlockSize -= 1;

                        if (maxBlockSize == 0)
                        {
                            maxBlockSize = -1;
                            break;
                        }

                        blockSize = GetBlockSize(maxBlockSize);
                    }

                    if (maxBlockSize == -1)
                    {
                        column += 1;
                        continue;
                    }

                    int index = _random.Next(imageFiles.Length - 1);
                    string imagePath = imageFiles[index];
                    BitmapImage bitmap = MemoryCache.Default.Get(imagePath) as BitmapImage;
                    if (bitmap == null)
                    {
                        try
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                            //bitmap.DecodePixelHeight = 3 * SquareSize;
                            //bitmap.DecodePixelWidth = 3 * SquareSize;
                            //bitmap.DecodePixelHeight = blockSize * SquareSize;
                            bitmap.DecodePixelWidth = blockSize * SquareSize;
                            bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                            //bitmap.CacheOption = BitmapCacheOption.OnDemand;
                            bitmap.EndInit();
                            bitmap.Freeze();

                            CacheItemPolicy policy = new CacheItemPolicy();
                            policy.SlidingExpiration = TimeSpan.FromMinutes(2);
                            MemoryCache.Default.Add(imagePath, bitmap, policy);
                        }
                        catch (Exception)
                        {
                            // Swallow
                        }
                    }

                    ImageMapEntry entry = new ImageMapEntry();
                    entry.Size = blockSize;
                    entry.StartRow = row;
                    entry.StartColumn = column;
                    entry.Bitmap = bitmap;

                    for (int r = row; r < row + blockSize; r++)
                    {
                        for (int c = column; c < column + blockSize; c++)
                        {
                            map[new KeyValuePair<int, int>(r, c)] = entry;
                        }
                    }

                    column += blockSize;
                }
            }

            return map.Values.Distinct();
        }

        private int GetBlockSize(int maxBlockSize)
        {
            double x = _random.NextDouble();
            double sum = 0;

            for (int i = 0; i < maxBlockSize; i++)
            {
                sum += _sizeDistribution[i];
                if (sum > x)
                {
                    return _sizeElements[i];
                }
            }

            return 1;
        }

        private void CreateMap(IEnumerable<ImageMapEntry> map)
        {
            foreach (ImageMapEntry entry in map.AsParallel())
            {
                Dispatcher.BeginInvoke((Action<ImageMapEntry>)AddImage, DispatcherPriority.ContextIdle, entry);
            }
        }

        private void AddImage(ImageMapEntry entry)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Height = entry.Size * SquareSize;
            rectangle.Width = entry.Size * SquareSize;
            rectangle.Opacity = 0.0;
            rectangle.RenderTransform = new ScaleTransform(0.7, 0.7);
            rectangle.RenderTransformOrigin = new Point(0.5, 0.5);
            rectangle.Loaded += delegate
            {
                var animationDuration = new Duration(TimeSpan.FromMilliseconds(700));

                DoubleAnimation opacityAnimation = new DoubleAnimation();
                opacityAnimation.To = 1.0;
                opacityAnimation.Duration = animationDuration;

                DoubleAnimation scaleXAnimation = new DoubleAnimation();
                scaleXAnimation.To = 1.0;
                scaleXAnimation.Duration = animationDuration;
                scaleXAnimation.EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseIn
                };

                DoubleAnimation scaleYAnimation = new DoubleAnimation();
                scaleYAnimation.To = 1.0;
                scaleYAnimation.Duration = animationDuration;
                scaleYAnimation.EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseIn
                };

                Timeline.SetDesiredFrameRate(opacityAnimation, 10);
                Timeline.SetDesiredFrameRate(scaleXAnimation, 10);
                Timeline.SetDesiredFrameRate(scaleYAnimation, 10);

                Storyboard.SetTarget(opacityAnimation, rectangle);
                Storyboard.SetTarget(scaleXAnimation, rectangle);
                Storyboard.SetTarget(scaleYAnimation, rectangle);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
                Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.ScaleX"));
                Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.ScaleY"));

                Storyboard s = new Storyboard();
                s.BeginTime = TimeSpan.FromMilliseconds(_random.Next(0, 500));
                s.Children.Add(opacityAnimation);
                s.Children.Add(scaleXAnimation);
                s.Children.Add(scaleYAnimation);
                s.Begin();
            };

            if (entry.Bitmap != null)
            {
                rectangle.Fill = new ImageBrush(entry.Bitmap)
                {
                    Stretch = Stretch.UniformToFill
                };
            }

            rectangle.Stroke = Brushes.Black;
            rectangle.StrokeThickness = 1.5;
            Canvas.SetLeft(rectangle, entry.StartColumn * SquareSize);
            Canvas.SetTop(rectangle, entry.StartRow * SquareSize);
            _canvas.Children.Add(rectangle);
        }

        #endregion Methods

        #region Nested Types

        private class ImageMapEntry
        {
            #region Fields

            public BitmapSource Bitmap;
            public int Size;
            public int StartColumn;
            public int StartRow;

            #endregion Fields

            #region Methods

            public override string ToString()
            {
                return string.Format("Size: {0}, StartColumn: {1}, StartRow: {2}", Size, StartColumn, StartRow);
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}