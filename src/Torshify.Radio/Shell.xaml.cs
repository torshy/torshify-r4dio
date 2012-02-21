using System.ComponentModel.Composition;

using AppLimit.NetSparkle;

using Torshify.Radio.Framework.Controls;
using Torshify.Radio.Properties;

namespace Torshify.Radio
{
    [Export(typeof(Shell))]
    public partial class Shell : MetroWindow
    {
        #region Fields

        private Sparkle _sparkle;

        #endregion Fields

        #region Constructors

        public Shell()
        {
            InitializeComponent();

            _sparkle = new Sparkle(Settings.Default.VersionInfoUri);
            _sparkle.StartLoop(true, true);
        }

        #endregion Constructors
    }
}