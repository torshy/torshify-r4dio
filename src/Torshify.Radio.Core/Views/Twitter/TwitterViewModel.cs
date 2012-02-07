using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Core.Views.Twitter
{
    public class Tweet
    {

    }

    [Export]
    [RegionMemberLifetime(KeepAlive = false)]
    public class TwitterViewModel : NotificationObject
    {
        #region Fields

        private ObservableCollection<Tweet> _tweets;

        #endregion Fields

        #region Constructors

        public TwitterViewModel()
        {
            _tweets = new ObservableCollection<Tweet>();
        }

        #endregion Constructors

        #region Properties

        public ObservableCollection<Tweet> Tweets
        {
            get { return _tweets; }
        }

        #endregion Properties
    }
}