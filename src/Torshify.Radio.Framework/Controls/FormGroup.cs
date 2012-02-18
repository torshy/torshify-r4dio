using System.Windows;
using System.Windows.Controls;

namespace Torshify.Radio.Framework.Controls
{
    public class FormGroup : HeaderedItemsControl
    {
        #region Fields

        public static readonly DependencyProperty ColumnsProperty = 
            DependencyProperty.Register("Columns", typeof(int), typeof(FormGroup),
                new FrameworkPropertyMetadata(1));

        #endregion Fields

        #region Constructors

        static FormGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormGroup), new FrameworkPropertyMetadata(typeof(FormGroup)));
        }

        #endregion Constructors

        #region Properties

        public int Columns
        {
            get
            {
                return (int)GetValue(ColumnsProperty);
            }
            set
            {
                SetValue(ColumnsProperty, value);
            }
        }

        #endregion Properties
    }
}