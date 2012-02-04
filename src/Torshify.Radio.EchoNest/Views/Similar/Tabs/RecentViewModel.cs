using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using Raven.Client;
using Torshify.Radio.Framework;
using System.Linq;

namespace Torshify.Radio.EchoNest.Views.Similar.Tabs
{
    [Export(typeof(RecentViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RecentViewModel : NotificationObject, IHeaderInfoProvider<HeaderInfo>
    {
        private ObservableCollection<SimilarArtistModel> _recentArtists;

        #region Constructors

        public RecentViewModel()
        {
            _recentArtists = new ObservableCollection<SimilarArtistModel>();

            HeaderInfo = new HeaderInfo();
            HeaderInfo.Title = "Recent";
            HeaderInfo.IsSelectedAction = IsSelectedAction;
        }

        #endregion Constructors

        #region Properties

        [Import]
        public IDocumentStore DocumentStore
        {
            get;
            set;
        }

        [Import]
        public ILoggerFacade Logger
        {
            get; 
            set;
        }

        public HeaderInfo HeaderInfo
        {
            get;
            private set;
        }

        public ObservableCollection<SimilarArtistModel> RecentArtists
        {
            get
            {
                return _recentArtists;
            }
        }

        #endregion Properties

        #region Methods

        private void IsSelectedAction(bool isSelected)
        {
            if (isSelected)
            {
                var ui = TaskScheduler.FromCurrentSynchronizationContext();
                Task
                    .Factory
                    .StartNew(() => FetchRecentArtists())
                    .ContinueWith(t =>
                                  {
                                      if (t.Exception != null)
                                      {
                                          Logger.Log(t.Exception.ToString(), Category.Exception, Priority.Medium);
                                      }
                                      else
                                      {
                                          PresentRecentArtists(t.Result);
                                      }
                                  }, ui);
            }
        }

        private SimilarArtistModel[] FetchRecentArtists()
        {
            using (var session = DocumentStore.OpenSession())
            {
                return session.Query<SimilarArtistModel>().ToArray();
            }
        }

        private void PresentRecentArtists(SimilarArtistModel[] models)
        {
            _recentArtists.Clear();

            foreach (var model in models)
            {
                _recentArtists.Add(model);
            }
        }

        #endregion Methods
    }
}