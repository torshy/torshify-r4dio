using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Threading;

namespace Torshify.Radio
{
    [Export]
    public class MefExports
    {
        #region Properties

        [Export]
        public Dispatcher Dispatcher
        {
            get { return Application.Current.Dispatcher; }
        }

        #endregion Properties
    }
}