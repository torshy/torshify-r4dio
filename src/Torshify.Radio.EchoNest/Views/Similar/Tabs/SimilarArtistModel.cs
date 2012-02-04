using System.Collections.Generic;

using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest.Views.Similar.Tabs
{
    public class SimilarArtistModel : NotificationObject
    {
        #region Properties

        public string Image
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public IEnumerable<string> Terms
        {
            get;
            set;
        }

        #endregion Properties
    }
}