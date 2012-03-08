using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Torshify.Radio.Framework.Controls
{
    public class AnimatingTilePanel : AnimatingPanel
    {
        #region Fields

        public static readonly DependencyProperty ItemHeightProperty = 
            CreateDoubleDP("ItemHeight", 50, FrameworkPropertyMetadataOptions.AffectsMeasure, 0, double.PositiveInfinity, true);
        public static readonly DependencyProperty ItemWidthProperty = 
            CreateDoubleDP("ItemWidth", 50, FrameworkPropertyMetadataOptions.AffectsMeasure, 0, double.PositiveInfinity, true);

        private bool _appliedTemplate;
        private bool _arrangedOnce;
        private DoubleAnimation _itemOpacityAnimation;

        #endregion Fields

        #region Properties

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        #endregion Properties

        #region Methods

        public static double GetItemWidth(DependencyObject element)
        {
            return (double)element.GetValue(ItemWidthProperty);
        }

        public static void SetItemWidth(DependencyObject element, double itemWidth)
        {
            element.SetValue(ItemWidthProperty, itemWidth);
        }

        public static double GetItemHeight(DependencyObject element)
        {
            return (double)element.GetValue(ItemHeightProperty);
        }

        public static void SetItemHeight(DependencyObject element, double itemHeight)
        {
            element.SetValue(ItemHeightProperty, itemHeight);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            onPreApplyTemplate();

            Size theChildSize = GetItemSize();

            foreach (UIElement child in Children)
            {
                child.Measure(theChildSize);
            }

            int childrenPerRow;

            // Figure out how many children fit on each row
            if (Double.IsPositiveInfinity(availableSize.Width))
            {
                childrenPerRow = Children.Count;
            }
            else
            {
                childrenPerRow = Math.Max(1, (int)Math.Floor(availableSize.Width / this.ItemWidth));
            }

            // Calculate the width and height this results in
            double width = childrenPerRow * this.ItemWidth;
            double height = ItemHeight * (Math.Floor((double)this.Children.Count / childrenPerRow) + 1);
            height = IsValid(height) ? height : 0;
            return new Size(width, height);
        }

        protected override sealed Size ArrangeOverride(Size finalSize)
        {
            // Calculate how many children fit on each row
            int childrenPerRow = Math.Max(1, (int)Math.Floor(finalSize.Width / this.ItemWidth));
            Size theChildSize = GetItemSize();

            for (int i = 0; i < this.Children.Count; i++)
            {
                // Figure out where the child goes
                Point newOffset = CalculateChildOffset(i, childrenPerRow,
                    this.ItemWidth, this.ItemHeight,
                    finalSize.Width, this.Children.Count);

                ArrangeChild(Children[i], new Rect(newOffset, theChildSize));
            }

            _arrangedOnce = true;
            return finalSize;
        }

        protected override Point ProcessNewChild(UIElement child, Rect providedBounds)
        {
            var startLocation = providedBounds.Location;
            if (_arrangedOnce)
            {
                if (_itemOpacityAnimation == null)
                {
                    _itemOpacityAnimation = new DoubleAnimation()
                    {
                        From = 0,
                        Duration = new Duration(TimeSpan.FromSeconds(.5))
                    };
                    _itemOpacityAnimation.Freeze();
                }

                child.BeginAnimation(UIElement.OpacityProperty, _itemOpacityAnimation);
                startLocation -= new Vector(providedBounds.Width, 0);
            }
            return startLocation;
        }

        // Given a child index, child size and children per row, figure out where the child goes
        private static Point CalculateChildOffset(
            int index,
            int childrenPerRow,
            double itemWidth,
            double itemHeight,
            double panelWidth,
            int totalChildren)
        {
            double fudge = 0;
            if (totalChildren > childrenPerRow)
            {
                fudge = (panelWidth - childrenPerRow * itemWidth) / childrenPerRow;
                Debug.Assert(fudge >= 0);
            }

            int row = index / childrenPerRow;
            int column = index % childrenPerRow;
            return new Point(.5 * fudge + column * (itemWidth + fudge), row * itemHeight);
        }

        private Size GetItemSize()
        {
            return new Size(ItemWidth, ItemHeight);
        }

        private void BindToParentItemsControl(DependencyProperty property, DependencyObject source)
        {
            if (DependencyPropertyHelper.GetValueSource(this, property).BaseValueSource == BaseValueSource.Default)
            {
                Binding binding = new Binding();
                binding.Source = source;
                binding.Path = new PropertyPath(property);
                base.SetBinding(property, binding);
            }
        }

        private void onPreApplyTemplate()
        {
            if (!_appliedTemplate)
            {
                _appliedTemplate = true;

                DependencyObject source = base.TemplatedParent;
                if (source is ItemsPresenter)
                {
                    source = TreeHelpers.FindParent<ItemsControl>(source);
                }

                if (source != null)
                {
                    BindToParentItemsControl(ItemHeightProperty, source);
                    BindToParentItemsControl(ItemWidthProperty, source);
                }
            }
        }

        private bool IsValid(double value)
        {
            return !double.IsInfinity(value) && !double.IsNaN(value);
        }
          
        #endregion Methods
    }
}