using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Raven.Client;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.EchoNest.Views.Similar.Tabs
{
    [Export(typeof(RecentViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RecentViewModel : NotificationObject, IHeaderInfoProvider<HeaderInfo>
    {
        #region Fields

        private ObservableCollection<SimilarArtistModel> _recentArtists;

        #endregion Fields

        #region Constructors

        public RecentViewModel()
        {
            _recentArtists = new ObservableCollection<SimilarArtistModel>();

            HeaderInfo = new HeaderInfo();
            HeaderInfo.Title = "Recent";
            HeaderInfo.IsSelectedAction = IsSelectedAction;
            PlayArtistCommand = new StaticCommand<SimilarArtistModel>(ExecutePlayArtist);
            QueueArtistCommand = new StaticCommand<SimilarArtistModel>(ExecuteQueueSimilarArtist);
            AddFavoriteArtistCommand = new StaticCommand<SimilarArtistModel>(ExecuteAddFavoriteArtist);
            SearchForSimilarArtistCommand = new StaticCommand<SimilarArtistModel>(ExecuteSearchForSimilarArtist);
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

        [Import]
        public IRadio Radio
        {
            get;
            set;
        }

        [Import]
        public IToastService ToastService
        {
            get;
            set;
        }

        [Import]
        public IRegionManager RegionManager
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

        public StaticCommand<SimilarArtistModel> PlayArtistCommand
        {
            get;
            private set;
        }

        public StaticCommand<SimilarArtistModel> QueueArtistCommand
        {
            get;
            private set;
        }

        public StaticCommand<SimilarArtistModel> AddFavoriteArtistCommand
        {
            get;
            private set;
        }

        public StaticCommand<SimilarArtistModel> SearchForSimilarArtistCommand
        {
            get;
            private set;
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

        private void ExecuteSearchForSimilarArtist(SimilarArtistModel artist)
        {
            UriQuery q = new UriQuery();
            q.Add(SearchBar.IsFromSearchBarParameter, "true");
            q.Add(SearchBar.ValueParameter, artist.Name);
            RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof(MainStationView).FullName + q);
        }

        private void ExecuteAddFavoriteArtist(SimilarArtistModel artist)
        {
        }

        private void ExecutePlayArtist(SimilarArtistModel artist)
        {
            Radio.Play(new SimilarArtistsTrackStream(Radio, new[] { artist.Name })
            {
                Description = artist.Name + " radio"
            });
        }

        private void ExecuteQueueSimilarArtist(SimilarArtistModel artist)
        {
            Radio.Queue(new SimilarArtistsTrackStream(Radio, new[] { artist.Name })
            {
                Description = artist.Name + " radio"
            });

            ToastService.Show("Queued " + artist.Name);
        }

        #endregion Methods
    }
}