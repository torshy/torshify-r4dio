using System.Collections.Generic;
using EchoNest.Artist;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest.Views.Similar.Tabs
{
    public class SimilarArtistModel : NotificationObject
    {
        #region Properties

        public ArtistBucketItem BucketItem
        {
            get;
            set;
        }

        public ImageItem Image
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public IEnumerable<TermsItem> Terms
        {
            get;
            set;
        }

        #endregion Properties
    }
}