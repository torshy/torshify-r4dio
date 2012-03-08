using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Torshify.Radio.Framework.Common;

namespace Torshify.Radio.Framework.Controls
{
    public abstract class AnimatingPanel : Panel
    {
        #region Fields

        public static readonly DependencyProperty AttractionProperty = 
            CreateDoubleDP("Attraction", 2, FrameworkPropertyMetadataOptions.None, 0, double.PositiveInfinity, false);
        public static readonly DependencyProperty DampeningProperty = 
            CreateDoubleDP("Dampening", 0.2, FrameworkPropertyMetadataOptions.None, 0, 1, false);
        public static readonly DependencyProperty VariationProperty = 
            CreateDoubleDP("Variation", 1, FrameworkPropertyMetadataOptions.None, 0, true, 1, true, false);

        private const double _diff = 0.1;
        private const double _terminalVelocity = 10000;

        private static readonly DependencyProperty DataProperty = 
            DependencyProperty.RegisterAttached("Data", typeof(AnimatingPanelItemData), typeof(AnimatingTilePanel));
        private static readonly WeakReference _random = new WeakReference(null);

        private readonly CompositionTargetRenderingListener _listener = new CompositionTargetRenderingListener();

        #endregion Fields

        #region Constructors

        protected AnimatingPanel()
        {
            _listener.Rendering += CompositionTargetRendering;
            _listener.WireParentLoadedUnloaded(this);
        }

        #endregion Constructors

        #region Properties

        public static Random Rnd
        {
            get
            {
                var r = (Random)_random.Target;
                if (r == null)
                {
                    _random.Target = r = new Random();
                }
                return r;
            }
        }

        public double Dampening
        {
            get { return (double)GetValue(DampeningProperty); }
            set { SetValue(DampeningProperty, value); }
        }

        public double Attraction
        {
            get { return (double)GetValue(AttractionProperty); }
            set { SetValue(AttractionProperty, value); }
        }

        public double Variation
        {
            get { return (double)GetValue(VariationProperty); }
            set { SetValue(VariationProperty, value); }
        }

        #endregion Properties

        #region Methods

        public static void SetToVector(TranslateTransform translateTransform, Vector vector)
        {
            translateTransform.X = vector.X;
            translateTransform.Y = vector.Y;
        }

        protected static DependencyProperty CreateDoubleDP(
          string name,
          double defaultValue,
          FrameworkPropertyMetadataOptions metadataOptions,
          double minValue,
          double maxValue,
          bool attached)
        {
            return CreateDoubleDP(name, defaultValue, metadataOptions, minValue, false, maxValue, false, attached);
        }

        protected static DependencyProperty CreateDoubleDP(
            string name,
            double defaultValue,
            FrameworkPropertyMetadataOptions metadataOptions,
            double minValue,
            bool includeMin,
            double maxValue,
            bool includeMax,
            bool attached)
        {
            ValidateValueCallback validateValueCallback = delegate(object objValue)
            {
                double value = (double)objValue;

                if (includeMin)
                {
                    if (value < minValue)
                    {
                        return false;
                    }
                }
                else
                {
                    if (value <= minValue)
                    {
                        return false;
                    }
                }
                if (includeMax)
                {
                    if (value > maxValue)
                    {
                        return false;
                    }
                }
                else
                {
                    if (value >= maxValue)
                    {
                        return false;
                    }
                }

                return true;
            };

            if (attached)
            {
                return DependencyProperty.RegisterAttached(
                    name,
                    typeof(double),
                    typeof(AnimatingTilePanel),
                    new FrameworkPropertyMetadata(defaultValue, metadataOptions), validateValueCallback);
            }

            return DependencyProperty.Register(
                name,
                typeof(double),
                typeof(AnimatingTilePanel),
                new FrameworkPropertyMetadata(defaultValue, metadataOptions), validateValueCallback);
        }

        protected virtual Point ProcessNewChild(UIElement child, Rect providedBounds)
        {
            return providedBounds.Location;
        }

        protected void ArrangeChild(UIElement child, Rect bounds)
        {
            _listener.StartListening();

            AnimatingPanelItemData data = (AnimatingPanelItemData)child.GetValue(DataProperty);
            if (data == null)
            {
                data = new AnimatingPanelItemData();
                child.SetValue(DataProperty, data);
                Debug.Assert(child.RenderTransform == Transform.Identity);
                child.RenderTransform = data.Transform;

                data.Current = ProcessNewChild(child, bounds);
            }
            Debug.Assert(child.RenderTransform == data.Transform);

            //set the location attached DP
            data.Target = bounds.Location;

            child.Arrange(bounds);
        }

        private static bool UpdateChildData(AnimatingPanelItemData data, double dampening, double attractionFactor, double variation)
        {
            if (data == null)
            {
                return false;
            }
            else
            {
                Debug.Assert(dampening > 0 && dampening < 1);
                Debug.Assert(attractionFactor > 0 && !double.IsInfinity(attractionFactor));

                attractionFactor *= 1 + (variation * data.RandomSeed - .5);

                Point newLocation;
                Vector newVelocity;

                bool anythingChanged =
                    GeoHelper.Animate(data.Current, data.LocationVelocity, data.Target,
                        attractionFactor, dampening, _terminalVelocity, _diff, _diff,
                        out newLocation, out newVelocity);

                data.Current = newLocation;
                data.LocationVelocity = newVelocity;

                var transformVector = data.Current - data.Target;
                SetToVector(data.Transform, transformVector);

                return anythingChanged;
            }
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            double dampening = Dampening;
            double attractionFactor = Attraction * .01;
            double variation = Variation;

            bool shouldChange = false;
            for (int i = 0; i < Children.Count; i++)
            {
                shouldChange = UpdateChildData(
                    (AnimatingPanelItemData)Children[i].GetValue(DataProperty),
                    dampening,
                    attractionFactor,
                    variation) || shouldChange;
            }

            if (!shouldChange)
            {
                _listener.StopListening();
            }
        }

        #endregion Methods

        #region Nested Types

        private class AnimatingPanelItemData
        {
            #region Fields

            public readonly double RandomSeed = Rnd.NextDouble();
            public readonly TranslateTransform Transform = new TranslateTransform();

            public Point Current;
            public Vector LocationVelocity;
            public Point Target;

            #endregion Fields
        }

        #endregion Nested Types
    }
}