using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Torshify.Radio.Framework.Controls
{
    public class DragPreviewAdorner : Adorner
    {
        #region Fields

        public static readonly DependencyProperty OffsetXProperty = 
            DependencyProperty.Register(
                "OffsetX", 
                typeof(double), 
                typeof(DragPreviewAdorner),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty OffsetYProperty = 
            DependencyProperty.Register(
                "OffsetY", 
                typeof(double), 
                typeof(DragPreviewAdorner),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        private ImageSource _dragImage;

        #endregion Fields

        #region Constructors

        public DragPreviewAdorner(UIElement adornedElement, FrameworkElement previewElement)
            : base(adornedElement)
        {
            SetPreviewElement(previewElement);
            DropShadowEffect d = new DropShadowEffect();
            d.BlurRadius = 25.0;
            d.ShadowDepth = 2;
            d.Opacity = 0.5;
            Effect = d;
        }

        #endregion Constructors

        #region Properties

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        #endregion Properties

        #region Methods

        public void SetPreviewElement(FrameworkElement element)
        {
            double x = 0;
            double y = 0;
            GetCurrentDPI(out x, out y);

            // Does the DrawingBrush thing seem a little hacky? Yeah, it's a necessary hack.  See:
            // http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=364041

            Rect bounds = VisualTreeHelper.GetDescendantBounds(element);

            RenderTargetBitmap r = new RenderTargetBitmap((int)(element.ActualWidth * x / 96), (int)(element.ActualHeight * y / 96), (int)x, (int)y, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);

                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }
            r.Render(dv);

            _dragImage = r;

            InvalidateMeasure();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawImage(_dragImage, new Rect(OffsetX, OffsetY, _dragImage.Width, _dragImage.Height));
        }

        private static void GetCurrentDPI(out double x, out double y)
        {
            Matrix m = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;
            x = 96 / m.M11;
            y = 96 / m.M22;
        }

        #endregion Methods
    }
}