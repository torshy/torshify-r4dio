using System;
using System.Windows;
using System.Windows.Controls;

namespace Torshify.Radio.Framework.Controls
{
    public class FormPanel : Panel
    {
        #region Fields

        public static readonly DependencyProperty ColumnSpacingProperty = 
            DependencyProperty.Register("ColumnSpacing", typeof(double), typeof(FormPanel),
            new FrameworkPropertyMetadata(15.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty ColumnsProperty = 
            DependencyProperty.Register("Columns", typeof(int), typeof(FormPanel),
            new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty ControlSizeProperty = 
            DependencyProperty.Register("ControlSize",typeof(Size),typeof(FormPanel));
        public static readonly DependencyProperty LabelControlSpacingProperty = 
            DependencyProperty.Register("LabelControlSpacing", typeof(double), typeof(FormPanel),
            new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty LabelSizeProperty = 
            DependencyProperty.Register("LabelSize", typeof(Size), typeof(FormPanel));
        public static readonly DependencyProperty RowSpacingProperty = 
            DependencyProperty.Register("RowSpacing", typeof(double), typeof(FormPanel),
            new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion Fields

        #region Properties

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public Size LabelSize
        {
            get { return (Size)GetValue(LabelSizeProperty); }
            set { SetValue(LabelSizeProperty, value); }
        }

        public Size ControlSize
        {
            get { return (Size)GetValue(ControlSizeProperty); }
            set { SetValue(ControlSizeProperty, value); }
        }

        public double ColumnSpacing
        {
            get { return (double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        public double RowSpacing
        {
            get { return (double)GetValue(RowSpacingProperty); }
            set { SetValue(RowSpacingProperty, value); }
        }

        public double LabelControlSpacing
        {
            get { return (double)GetValue(LabelControlSpacingProperty); }
            set { SetValue(LabelControlSpacingProperty, value); }
        }

        public IFormPanelCoordinator Coordinator
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            double labelMaxWidth = 0;
            double labelMaxHeight = 0;
            double controlMaxWidth = 0;
            double controlMaxHeight = 0;
            for (int i = 0; i < Children.Count-1; i += 2)
            {
                Children[i].Measure(availableSize);
                Children[i + 1].Measure(availableSize);
                labelMaxWidth = Math.Max(labelMaxWidth, Children[i].DesiredSize.Width);
                labelMaxHeight = Math.Max(labelMaxHeight, Children[i].DesiredSize.Height);
                controlMaxWidth = Math.Max(controlMaxWidth, Children[i+1].DesiredSize.Width);
                controlMaxHeight = Math.Max(controlMaxHeight, Children[i+1].DesiredSize.Height);
            }

            var oldLabelSize = LabelSize;
            var oldControlSize = ControlSize;
            var newLabelSize = new Size(labelMaxWidth, labelMaxHeight);
            var newControlSize = new Size(controlMaxWidth, controlMaxHeight);
            LabelSize = newLabelSize;
            ControlSize = newControlSize;

            if (Coordinator != null &&
                (newLabelSize != oldLabelSize || newControlSize != oldControlSize))
            {
                Coordinator.ControlOrLabelSizeChanged(this);
            }

            return new Size(
                Math.Max(0, Columns * (LabelSize.Width + ControlSize.Width + LabelControlSpacing) + (Columns - 1) * ColumnSpacing),
                Math.Max(0, ((Children.Count/2) / Columns) * Math.Max(LabelSize.Height, ControlSize.Height) + (((Children.Count/2) / Columns) - 1) * RowSpacing));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double controlWidth = (finalSize.Width - (Columns - 1) * ColumnSpacing - Columns * (LabelSize.Width + LabelControlSpacing)) / Columns;
            double rowHeight = Math.Max(LabelSize.Height, ControlSize.Height) + RowSpacing;
            double columnWidth = LabelSize.Width + LabelControlSpacing + controlWidth + ColumnSpacing;
            for (int i = 0; i < Children.Count - 1; i += 2)
            {
                var labelRect = new Rect(
                    columnWidth * ((i / 2) % Columns), rowHeight * ((i / 2) / Columns),
                    LabelSize.Width, rowHeight - RowSpacing);
                Children[i].Arrange(
                    new Rect(
                        labelRect.Left,
                        labelRect.Top+(labelRect.Height-Children[i].DesiredSize.Height)/2,
                        Children[i].DesiredSize.Width,Children[i].DesiredSize.Height));
                Children[i + 1].Arrange(new Rect(
                    columnWidth * ((i / 2) % Columns) + LabelSize.Width + LabelControlSpacing, rowHeight * ((i / 2) / Columns),
                    controlWidth, rowHeight - RowSpacing));
            }
            return new Size(finalSize.Width, rowHeight * ((Children.Count/2) / Columns + 1));
        }

        #endregion Methods
    }
}