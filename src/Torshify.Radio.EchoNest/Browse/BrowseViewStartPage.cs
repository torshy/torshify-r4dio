using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Torshify.Radio.EchoNest.Browse
{
    [Export(typeof(BrowseViewStartPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BrowseViewStartPage : UserControl
    {

    }
}