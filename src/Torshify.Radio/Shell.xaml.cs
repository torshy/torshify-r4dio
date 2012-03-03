using System.ComponentModel.Composition;
using System.Windows.Forms;

using AppLimit.NetSparkle;

using Torshify.Radio.Framework.Controls;
using Torshify.Radio.Properties;

namespace Torshify.Radio
{
    [Export(typeof(Shell))]
    public partial class Shell : MetroWindow, IMessageFilter
    {
        #region Fields

        private Sparkle _sparkle;
        private int _wmPaint = 0x000F;

        #endregion Fields

        #region Constructors

        public Shell()
        {
            InitializeComponent();

            Application.AddMessageFilter(this);

            _sparkle = new Sparkle(Settings.Default.VersionInfoUri);
            _sparkle.ApplicationIcon = Properties.Resources.r4dio_app.ToBitmap();
            _sparkle.ApplicationWindowIcon = Properties.Resources.r4dio_app;
            _sparkle.StartLoop(true, true);
        }

        #endregion Constructors

        #region Methods

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == _wmPaint)
            {
                var form = Control.FromHandle(m.HWnd) as Form;

                if (form != null && !form.AutoSize)
                {
                    form.AutoSize = true;
                }
            }

            return false;
        }

        #endregion Methods
    }
}