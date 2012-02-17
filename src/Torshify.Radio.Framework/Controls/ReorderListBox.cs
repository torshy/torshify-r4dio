using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Torshify.Radio.Framework.Controls
{
    #region Enumerations

    public enum ReorderPlacement
    {
        Before, After
    }

    public enum ReorderQuadrant
    {
        TopLeft, TopRight, BottomLeft, BottomRight
    }

    #endregion Enumerations

    public class ReorderEventArgs : RoutedEventArgs
    {
        #region Constructors

        public ReorderEventArgs(DependencyObject itemContainer, DependencyObject toContainer)
            : base(ReorderListBox.ReorderRequestedEvent)
        {
            ItemContainer = itemContainer;
            ToContainer = toContainer;
        }

        #endregion Constructors

        #region Properties

        public DependencyObject ItemContainer
        {
            get; private set;
        }

        public DependencyObject ToContainer
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return string.Format("{0} - From {1} to {2}", base.ToString(), ItemContainer, ToContainer);
        }

        #endregion Methods
    }

    public class ReorderListBox : ListBox
    {
        #region Fields

        public static readonly DependencyProperty DurationProperty = 
            DependencyProperty.Register(
                "Duration",
                typeof(int),
                typeof(ReorderListBox),
                new FrameworkPropertyMetadata((int)250));
        public static readonly DependencyProperty IsDragElementProperty = 
            DependencyProperty.RegisterAttached(
                "IsDragElement",
                typeof(bool),
                typeof(ReorderListBox),
                new FrameworkPropertyMetadata(false, OnIsDragElementChanged));
        public static readonly DependencyProperty IsDraggingProperty;
        public static readonly DependencyProperty OrientationProperty = DependencyPropHelper.Register<ReorderListBox, Orientation>("Orientation", Orientation.Vertical);
        public static readonly RoutedEvent ReorderBeginEvent = 
            EventManager.RegisterRoutedEvent(
                "ReorderBegin",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ReorderListBox));
        public static readonly RoutedEvent ReorderCancelEvent = 
            EventManager.RegisterRoutedEvent(
                "ReorderCancel",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ReorderListBox));
        public static readonly RoutedEvent ReorderRequestedEvent = 
            EventManager.RegisterRoutedEvent(
                "ReorderRequested",
                RoutingStrategy.Bubble,
                typeof(EventHandler<ReorderEventArgs>),
                typeof(ReorderListBox));
        private static readonly DependencyPropertyKey IsDraggingPropertyKey = 
            DependencyProperty.RegisterAttachedReadOnly(
                "IsDragging",
                typeof(bool),
                typeof(ReorderListBox),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
        private static readonly DependencyProperty _dragPreviewStoryboardProperty = 
            DependencyProperty.RegisterAttached(
                "Private_DragPreviewStoryboard",
                typeof(Storyboard),
                typeof(ReorderListBox),
                new FrameworkPropertyMetadata((Storyboard)null));

        private AdornerLayer _adornerLayerCache;
        private DragPreviewAdorner _dragAdorner;
        private int _dragInsertIndex;
        private FrameworkElement _dragItem;
        private int _dragItemIndex;
        private bool _isDragging;
        private FrameworkElement _lastMouseOverItem;
        private ReorderQuadrant _lastMouseOverQuadrant;
        private ReorderPlacement _lastMoveOverPlacement;
        private Point _mouseDown;

        #endregion Fields

        #region Constructors

        static ReorderListBox()
        {
            IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ReorderListBox), new FrameworkPropertyMetadata(typeof(ReorderListBox)));
        }

        #endregion Constructors

        #region Events

        public event RoutedEventHandler ReorderBegin
        {
            add { AddHandler(ReorderBeginEvent, value); }
            remove { RemoveHandler(ReorderBeginEvent, value); }
        }

        public event RoutedEventHandler ReorderCancel
        {
            add { AddHandler(ReorderCancelEvent, value); }
            remove { RemoveHandler(ReorderCancelEvent, value); }
        }

        public event EventHandler<ReorderEventArgs> ReorderRequested
        {
            add { AddHandler(ReorderRequestedEvent, value); }
            remove { RemoveHandler(ReorderRequestedEvent, value); }
        }

        #endregion Events

        #region Properties

        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        private AdornerLayer AdornerLayer
        {
            get
            {
                if (_adornerLayerCache == null)
                {
                    _adornerLayerCache = AdornerLayer.GetAdornerLayer(this);
                }

                return _adornerLayerCache;
            }
        }

        #endregion Properties

        #region Methods

        public static bool GetIsDragElement(DependencyObject d)
        {
            return (bool)d.GetValue(IsDragElementProperty);
        }

        public static void SetIsDragElement(DependencyObject d, bool value)
        {
            d.SetValue(IsDragElementProperty, value);
        }

        public static bool GetIsDragging(DependencyObject d)
        {
            return (bool)d.GetValue(IsDraggingProperty);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDragging && _dragAdorner != null)
            {
                // update the position of the adorner

                var current = e.GetPosition(this);
                _dragAdorner.OffsetX = current.X - _mouseDown.X;
                _dragAdorner.OffsetY = current.Y - _mouseDown.Y;

                // find the item that we are dragging over
                var element = InputHitTest(new Point(e.GetPosition(this).X, e.GetPosition(this).Y)) as UIElement;

                if (element != null)
                {
                    var itemOver = TreeHelpers.GetItemContainerFromChildElement(this, element) as FrameworkElement;

                    if (itemOver != null)
                    {
                        var p = Mouse.GetPosition(itemOver);
                        var q = PointToQuadrant(itemOver, p);

                        if (itemOver != _lastMouseOverItem || q != _lastMouseOverQuadrant)
                        {
                            if (q == ReorderQuadrant.BottomLeft || q == ReorderQuadrant.BottomRight)
                            {
                                _lastMoveOverPlacement = ReorderPlacement.After;
                            }
                            else
                            {
                                _lastMoveOverPlacement = ReorderPlacement.Before;
                            }
                            PreviewInsert(itemOver, _lastMoveOverPlacement);
                            _lastMouseOverItem = itemOver;
                            _lastMouseOverQuadrant = q;
                        }
                    }
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                ReleaseMouseCapture();
                AdornerLayer.Remove(_dragAdorner);
                _isDragging = false;

                // raise an event to update the underlying datasource
                if (_lastMouseOverItem != null && _dragItemIndex != _dragInsertIndex)
                {
                    var insertItem = ItemContainerGenerator.ContainerFromIndex(_dragInsertIndex);
                    OnReorderRequested(new ReorderEventArgs(_dragItem, insertItem));
                }
                else
                {
                    RaiseReorderCancelEvent();
                }

                _dragItem.ClearValue(IsDraggingPropertyKey);
                _dragItem.Visibility = Visibility.Visible;

                // If items are manually added, just re-order them.
                if (ItemsSource == null)
                {
                    Items.Remove(_dragItem);
                    Items.Insert(_dragInsertIndex, _dragItem);
                }

                // reset the transform on all of the items
                for (var i = 0; i < Items.Count; i++)
                {
                    var item = (FrameworkElement)ItemContainerGenerator.ContainerFromIndex(i);
                    if (item != null)
                    {
                        TranslateItem(item, 0, 0, Orientation);
                    }
                }

                e.Handled = true;
            }

            // clean-up dragging variables
            _dragInsertIndex = _dragItemIndex = int.MinValue;
            _dragItem = _lastMouseOverItem = null;

            base.OnMouseUp(e);
        }

        protected virtual void OnReorderRequested(ReorderEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// Handles changes to the IsDragElement property.
        /// </summary>
        private static void OnIsDragElementChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var element = (UIElement)obj;
            if ((bool)args.NewValue)
            {
                element.MouseLeftButtonDown += ElementMouseLeftButtonDown;
            }
            else
            {
                element.MouseLeftButtonDown -= ElementMouseLeftButtonDown;
            }
        }

        private static void ElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;

            // find the ReoderListBox parent of the element
            var reorderListBox = TreeHelpers.FindParent<ReorderListBox>(element);

            if (reorderListBox != null)
            {
                // find the ItemContainer
                FrameworkElement f = TreeHelpers.GetItemContainerFromChildElement(reorderListBox, element) as FrameworkElement;
                if (f != null)
                {
                    reorderListBox.BeginDrag(f);
                }
            }
        }

        /// <summary>
        /// Provides a secure method for setting the IsDragging property.
        /// This dependency property indicates ....
        /// </summary>
        private static void SetIsDragging(DependencyObject d, bool value)
        {
            d.SetValue(IsDraggingPropertyKey, value);
        }

        private static ReorderQuadrant PointToQuadrant(FrameworkElement element, Point p)
        {
            if (p.Y >= (element.ActualHeight / 2))
            {
                // top half
                if (p.X >= (element.ActualWidth / 2))
                {
                    return ReorderQuadrant.BottomRight;
                }

                return ReorderQuadrant.BottomLeft;
            }

            // bottom half
            if (p.X >= (element.ActualWidth / 2))
            {
                return ReorderQuadrant.TopRight;
            }

            return ReorderQuadrant.TopLeft;
        }

        private static void TranslateItem(FrameworkElement element, double delta, int milliseconds, Orientation orientation)
        {
            var storyboard = (Storyboard)element.GetValue(_dragPreviewStoryboardProperty);
            SplineDoubleKeyFrame keyframe;

            if (storyboard == null)
            {
                var t = new TranslateTransform();
                element.RenderTransform = t;

                keyframe = new SplineDoubleKeyFrame
                {
                    KeySpline = new KeySpline(0, 0.7, 0.7, 1)
                };
                var animation = new DoubleAnimationUsingKeyFrames();
                animation.KeyFrames.Add(keyframe);

                Storyboard.SetTarget(animation, element);
                PropertyPath propertyPath;
                if (orientation == Orientation.Vertical)
                {
                    propertyPath = new PropertyPath("(RenderTransform).(TranslateTransform.Y)");
                }
                else
                {
                    propertyPath = new PropertyPath("(RenderTransform).(TranslateTransform.X)");
                }
                Storyboard.SetTargetProperty(animation, propertyPath);

                storyboard = new Storyboard();
                storyboard.Children.Add(animation);

                element.SetValue(_dragPreviewStoryboardProperty, storyboard);
            }
            else
            {
                keyframe = storyboard.Children.Cast<DoubleAnimationUsingKeyFrames>().Single().KeyFrames.Cast<SplineDoubleKeyFrame>().Single();
            }

            keyframe.Value = delta;
            keyframe.KeyTime = TimeSpan.FromMilliseconds(milliseconds);
            element.BeginStoryboard(storyboard);
        }

        private void RaiseReorderBeginEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ReorderBeginEvent);
            RaiseEvent(newEventArgs);
        }

        private void RaiseReorderCancelEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ReorderCancelEvent);
            RaiseEvent(newEventArgs);
        }

        private void BeginDrag(FrameworkElement dragContainer)
        {
            _dragItem = dragContainer;
            if (_dragItem == null) return;

            // get the index of the item (and make sure that it is a valid child)
            _dragItemIndex = this.ItemContainerGenerator.IndexFromContainer(_dragItem);
            if (_dragItemIndex == -1) return;

            // create an adorner
            _dragAdorner = new DragPreviewAdorner(_dragItem, _dragItem);
            _dragAdorner.IsHitTestVisible = false;
            this.AdornerLayer.Add(_dragAdorner);

            // tell the item it's dragging and hide it
            SetIsDragging(_dragItem, true);
            _dragItem.Visibility = Visibility.Hidden;

            // get the current location of the mouse
            _mouseDown = Mouse.GetPosition(this);

            // set mouse capture (so that we are dragging)
            Mouse.Capture(this);
            _isDragging = true;

            // raise an event to signal that we've started ragging
            RaiseReorderBeginEvent();
        }

        private void PreviewInsert(FrameworkElement relativeTo, ReorderPlacement placement)
        {
            if (_isDragging && _dragItem != null && relativeTo != null)
            {
                // get the index of the item being dragged
                var relativeToIndex = ItemContainerGenerator.IndexFromContainer(relativeTo);

                // get the index of insertion
                var offset = (placement == ReorderPlacement.Before) ? 0 : 1;
                _dragInsertIndex = relativeToIndex + offset;

                for (int i = 0; i < Items.Count; i++)
                {
                    double delta;
                    if (i > _dragItemIndex && i < _dragInsertIndex)
                    {
                        delta = -1 * GetOrientedDimension(_dragItem);
                    }
                    else if (i < _dragItemIndex && i >= _dragInsertIndex)
                    {
                        delta = GetOrientedDimension(_dragItem);
                    }
                    else
                    {
                        delta = 0;
                    }

                    TranslateItem((FrameworkElement)ItemContainerGenerator.ContainerFromIndex(i), delta, this.Duration, Orientation);
                }

                // if the insert location is after the current location, we need to decrement it
                // by one after we've made the visual adjustments so that the actual drop index
                // will be accurate
                if (_dragInsertIndex > _dragItemIndex)
                {
                    _dragInsertIndex--;
                }
            }
        }

        private double GetOrientedDimension(FrameworkElement element)
        {
            switch (Orientation)
            {
                case Orientation.Vertical:
                    return element.ActualHeight;
                case Orientation.Horizontal:
                    return element.ActualWidth;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion Methods
    }
}