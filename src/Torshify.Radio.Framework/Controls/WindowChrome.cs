#region Header

/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

#endregion Header

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;

using Torshify.Radio.Framework.Controls.Standard;

namespace Torshify.Radio.Framework.Controls
{
    #region Enumerations

    public enum ResizeGripDirection
    {
        None,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
    }

    [Flags]
    public enum SacrificialEdge
    {
        None = 0,
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,

        Office = Left | Right | Bottom,

        // Don't use "All" - Handling WM_NCCALCSIZE with a client rect shrunk in all directions implicitly creates a
        // normal sized caption area that doesn't actually properly participate with the rest of the implementation...
        // All = Left | Top | Right | Bottom,
    }

    #endregion Enumerations

    public class WindowChrome : Freezable
    {
        #region Fields

        public static readonly DependencyProperty CaptionHeightProperty = DependencyProperty.Register(
            "CaptionHeight",
            typeof(double),
            typeof(WindowChrome),
            new PropertyMetadata(
                0d,
                (d, e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint()),
            value => (double)value >= 0d);
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(WindowChrome),
            new PropertyMetadata(
                default(CornerRadius),
                (d, e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint()),
            (value) => Utility.IsCornerRadiusValid((CornerRadius)value));
        public static readonly DependencyProperty GlassFrameThicknessProperty = DependencyProperty.Register(
            "GlassFrameThickness",
            typeof(Thickness),
            typeof(WindowChrome),
            new PropertyMetadata(
                default(Thickness),
                (d, e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint(),
                (d, o) => _CoerceGlassFrameThickness((Thickness)o)));
        public static readonly DependencyProperty IsHitTestVisibleInChromeProperty = DependencyProperty.RegisterAttached(
            "IsHitTestVisibleInChrome",
            typeof(bool),
            typeof(WindowChrome),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
        public static readonly DependencyProperty ResizeBorderThicknessProperty = DependencyProperty.Register(
            "ResizeBorderThickness",
            typeof(Thickness),
            typeof(WindowChrome),
            new PropertyMetadata(default(Thickness)),
            (value) => Utility.IsThicknessNonNegative((Thickness)value));
        public static readonly DependencyProperty ResizeGripDirectionProperty = DependencyProperty.RegisterAttached(
            "ResizeGripDirection",
            typeof(ResizeGripDirection),
            typeof(WindowChrome),
            new FrameworkPropertyMetadata(ResizeGripDirection.None, FrameworkPropertyMetadataOptions.Inherits));
        public static readonly DependencyProperty SacrificialEdgeProperty = DependencyProperty.Register(
            "SacrificialEdge",
            typeof(SacrificialEdge),
            typeof(WindowChrome),
            new PropertyMetadata(
                SacrificialEdge.None,
                (d, e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint()),
                _IsValidSacrificialEdge);
        public static readonly DependencyProperty UseAeroCaptionButtonsProperty = DependencyProperty.Register(
            "UseAeroCaptionButtons",
            typeof(bool),
            typeof(WindowChrome),
            new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty WindowChromeProperty = DependencyProperty.RegisterAttached(
            "WindowChrome",
            typeof(WindowChrome),
            typeof(WindowChrome),
            new PropertyMetadata(null, _OnChromeChanged));

        private static readonly SacrificialEdge SacrificialEdge_All = SacrificialEdge.Bottom | SacrificialEdge.Top | SacrificialEdge.Left | SacrificialEdge.Right;
        private static readonly List<_SystemParameterBoundProperty> _BoundProperties = new List<_SystemParameterBoundProperty>
        {
            new _SystemParameterBoundProperty { DependencyProperty = CornerRadiusProperty, SystemParameterPropertyName = "WindowCornerRadius" },
            new _SystemParameterBoundProperty { DependencyProperty = CaptionHeightProperty, SystemParameterPropertyName = "WindowCaptionHeight" },
            new _SystemParameterBoundProperty { DependencyProperty = ResizeBorderThicknessProperty, SystemParameterPropertyName = "WindowResizeBorderThickness" },
            new _SystemParameterBoundProperty { DependencyProperty = GlassFrameThicknessProperty, SystemParameterPropertyName = "WindowNonClientFrameThickness" },
        };

        #endregion Fields

        #region Constructors

        public WindowChrome()
        {
            // Effective default values for some of these properties are set to be bindings
            // that set them to system defaults.
            // A more correct way to do this would be to Coerce the value iff the source of the DP was the default value.
            // Unfortunately with the current property system we can't detect whether the value being applied at the time
            // of the coersion is the default.
            foreach (var bp in _BoundProperties)
            {
                // This list must be declared after the DP's are assigned.
                Assert.IsNotNull(bp.DependencyProperty);
                BindingOperations.SetBinding(
                    this,
                    bp.DependencyProperty,
                    new Binding
                    {
                        Source = SystemParameters2.Current,
                        Path = new PropertyPath(bp.SystemParameterPropertyName),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    });
            }
        }

        #endregion Constructors

        #region Events

        internal event EventHandler PropertyChangedThatRequiresRepaint;

        #endregion Events

        #region Properties

        // Named property available for fully extending the glass frame.
        public static Thickness GlassFrameCompleteThickness
        {
            get { return new Thickness(-1); }
        }

        /// <summary>The extent of the top of the window to treat as the caption.</summary>
        public double CaptionHeight
        {
            get { return (double)GetValue(CaptionHeightProperty); }
            set { SetValue(CaptionHeightProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Thickness GlassFrameThickness
        {
            get { return (Thickness)GetValue(GlassFrameThicknessProperty); }
            set { SetValue(GlassFrameThicknessProperty, value); }
        }

        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
            set { SetValue(ResizeBorderThicknessProperty, value); }
        }

        public SacrificialEdge SacrificialEdge
        {
            get { return (SacrificialEdge)GetValue(SacrificialEdgeProperty); }
            set { SetValue(SacrificialEdgeProperty, value); }
        }

        public bool UseAeroCaptionButtons
        {
            get { return (bool)GetValue(UseAeroCaptionButtonsProperty); }
            set { SetValue(UseAeroCaptionButtonsProperty, value); }
        }

        #endregion Properties

        #region Methods

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool GetIsHitTestVisibleInChrome(IInputElement inputElement)
        {
            Verify.IsNotNull(inputElement, "inputElement");
            var dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            return (bool)dobj.GetValue(IsHitTestVisibleInChromeProperty);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static ResizeGripDirection GetResizeGripDirection(IInputElement inputElement)
        {
            Verify.IsNotNull(inputElement, "inputElement");
            var dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            return (ResizeGripDirection)dobj.GetValue(ResizeGripDirectionProperty);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static WindowChrome GetWindowChrome(Window window)
        {
            Verify.IsNotNull(window, "window");
            return (WindowChrome)window.GetValue(WindowChromeProperty);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetIsHitTestVisibleInChrome(IInputElement inputElement, bool hitTestVisible)
        {
            Verify.IsNotNull(inputElement, "inputElement");
            var dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            dobj.SetValue(IsHitTestVisibleInChromeProperty, hitTestVisible);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetResizeGripDirection(IInputElement inputElement, ResizeGripDirection direction)
        {
            Verify.IsNotNull(inputElement, "inputElement");
            var dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            dobj.SetValue(ResizeGripDirectionProperty, direction);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetWindowChrome(Window window, WindowChrome chrome)
        {
            Verify.IsNotNull(window, "window");
            window.SetValue(WindowChromeProperty, chrome);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new WindowChrome();
        }

        private static object _CoerceGlassFrameThickness(Thickness thickness)
        {
            // If it's explicitly set, but set to a thickness with at least one negative side then
            // coerce the value to the stock GlassFrameCompleteThickness.
            if (!Utility.IsThicknessNonNegative(thickness))
            {
                return GlassFrameCompleteThickness;
            }

            return thickness;
        }

        private static bool _IsValidSacrificialEdge(object value)
        {
            SacrificialEdge se = SacrificialEdge.None;
            try
            {
                se = (SacrificialEdge)value;
            }
            catch (InvalidCastException)
            {
                return false;
            }

            if (se == SacrificialEdge.None)
            {
                return true;
            }

            // Does this only contain valid bits?
            if ((se | SacrificialEdge_All) != SacrificialEdge_All)
            {
                return false;
            }

            // It can't sacrifice all 4 edges.  Weird things happen.
            if (se == SacrificialEdge_All)
            {
                return false;
            }

            return true;
        }

        private static void _OnChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // The different design tools handle drawing outside their custom window objects differently.
            // Rather than try to support this concept in the design surface let the designer draw its own
            // chrome anyways.
            // There's certainly room for improvement here.
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
            {
                return;
            }

            var window = (Window)d;
            var newChrome = (WindowChrome)e.NewValue;

            Assert.IsNotNull(window);

            // Update the ChromeWorker with this new object.

            // If there isn't currently a worker associated with the Window then assign a new one.
            // There can be a many:1 relationship of Window to WindowChrome objects, but a 1:1 for a Window and a WindowChromeWorker.
            WindowChromeWorker chromeWorker = WindowChromeWorker.GetWindowChromeWorker(window);
            if (chromeWorker == null)
            {
                chromeWorker = new WindowChromeWorker();
                WindowChromeWorker.SetWindowChromeWorker(window, chromeWorker);
            }

            chromeWorker.SetWindowChrome(newChrome);
        }

        private void _OnPropertyChangedThatRequiresRepaint()
        {
            var handler = PropertyChangedThatRequiresRepaint;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Methods

        #region Nested Types

        private struct _SystemParameterBoundProperty
        {
            #region Properties

            public DependencyProperty DependencyProperty
            {
                get; set;
            }

            public string SystemParameterPropertyName
            {
                get; set;
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}