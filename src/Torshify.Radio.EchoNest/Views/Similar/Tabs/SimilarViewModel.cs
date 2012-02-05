using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using EchoNest;
using EchoNest.Artist;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Raven.Client;
using Raven.Client.Linq;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.EchoNest.Views.Similar.Tabs
{
    [Export(typeof(SimilarViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SimilarViewModel : NotificationObject, INavigationAware, IHeaderInfoProvider<HeaderInfo>
    {
        #region Fields

        private string _currentMainArtist;
        private ObservableCollection<SimilarArtistModel> _similarArtists;

        #endregion Fields

        #region Constructors

        public SimilarViewModel()
        {
            _similarArtists = new ObservableCollection<SimilarArtistModel>();

            HeaderInfo = new HeaderInfo { Title = "Similar artists" };
            PlayArtistCommand = new StaticCommand<SimilarArtistModel>(ExecutePlaySimilarArtist);
            QueueArtistCommand = new StaticCommand<SimilarArtistModel>(ExecuteQueueSimilarArtist);
            AddFavoriteArtistCommand = new StaticCommand<SimilarArtistModel>(ExecuteAddFavoriteArtist);
        }

        #endregion Constructors

        #region Properties

        [Import]
        public IRadio Radio
        {
            get;
            set;
        }

        [Import]
        public ILoadingIndicatorService LoadingIndicatorService
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
        public ILoggerFacade Logger
        {
            get;
            set;
        }

        [Import]
        public IDocumentStore DocumentStore
        {
            get;
            set;
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

        public ObservableCollection<SimilarArtistModel> SimilarArtists
        {
            get
            {
                return _similarArtists;
            }
        }

        public HeaderInfo HeaderInfo
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        void INavigationAware.OnNavigatedTo(NavigationContext context)
        {
            if (!string.IsNullOrEmpty(context.Parameters[SearchBar.IsFromSearchBarParameter]))
            {
                string query = context.Parameters[SearchBar.ValueParameter];
                ExecuteGetSimilarArtists(query);
                ExecuteStoreRecentSimilarArtist(query);
            }
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext context)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext context)
        {
        }

        private static ArtistBucketItem GetArtistInformation(string query)
        {
            using (var session = new EchoNestSession(EchoNestModule.ApiKey))
            {
                var response = session
                    .Query<Profile>()
                    .Execute(query, ArtistBucket.Terms | ArtistBucket.Images);

                if (response.Status.Code == ResponseCode.Success)
                {
                    return response.Artist;
                }
            }

            return null;
        }

        private static SimilarArtistModel ConvertToModel(ArtistBucketItem bucket)
        {
            ImageItem image = null;
            TermsItem[] terms = null;

            if (bucket.Images != null)
            {
                image = bucket.Images.FirstOrDefault();
            }

            if (bucket.Terms != null)
            {
                terms = bucket.Terms.Take(2).ToArray();
            }

            return new SimilarArtistModel
                   {
                       Name = bucket.Name,
                       Image = image != null ? image.Url : null,
                       Terms = terms != null ? terms.Select(t => t.Name) : null
                   };
        }

        private void ExecuteStoreRecentSimilarArtist(string query)
        {
            Task
                .Factory
                .StartNew(state => GetArtistInformation(state.ToString()), query)
                .ContinueWith(task =>
                              {
                                  if (task.Exception != null)
                                  {
                                      Logger.Log(task.Exception.ToString(), Category.Exception, Priority.Low);
                                  }
                                  else
                                  {
                                      if (task.Result != null)
                                      {
                                          SaveRecentSimilarArtist(ConvertToModel(task.Result));
                                      }
                                  }
                              });
        }

        private void SaveRecentSimilarArtist(SimilarArtistModel artistModel)
        {
            try
            {
                using (var session = DocumentStore.OpenSession())
                {
                    IRavenQueryable<SimilarArtistModel> artists = session.Query<SimilarArtistModel>();

                    if (artists.Any(a => a.Name.Equals(artistModel.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        return;
                    }

                    int count = artists.Count();

                    if (count > 50)
                    {
                        var tooMany = count - 50;
                        var tooManyArtists = artists.Take(tooMany);

                        foreach (var artist in tooManyArtists)
                        {
                            session.Delete(artist);
                        }
                    }

                    session.Store(artistModel);
                    session.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, Category.Exception, Priority.Medium);
            }
        }

        private void ExecuteAddFavoriteArtist(SimilarArtistModel artist)
        {
        }

        private void ExecutePlaySimilarArtist(SimilarArtistModel artist)
        {
            Radio.Play(new SimilarArtistsTrackStream(Radio, new[] { artist.Name })
            {
                Description = "Similar artists of " + _currentMainArtist
            });
        }

        private void ExecuteQueueSimilarArtist(SimilarArtistModel artist)
        {
            Radio.Queue(new SimilarArtistsTrackStream(Radio, new[] { artist.Name })
            {
                Description = "Similar artists of " + _currentMainArtist
            });

            ToastService.Show("Queued " + artist.Name);
        }

        private void ExecuteGetSimilarArtists(string artistName)
        {
            _currentMainArtist = artistName;

            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task<IEnumerable<ArtistBucketItem>>
                .Factory
                .StartNew(state => GetSimilarArtists(state.ToString()), artistName)
                .ContinueWith(task =>
                              {
                                  if (task.Exception != null)
                                  {
                                      ToastService.Show("Unable to get similar artists");
                                      Logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                                  }
                                  else
                                  {
                                      PresentSimilarArtists(task.Result);
                                  }
                              }, ui);
        }

        private IEnumerable<ArtistBucketItem> GetSimilarArtists(string artistName)
        {
            using (LoadingIndicatorService.EnterLoadingBlock())
            {
                using (var session = new EchoNestSession(EchoNestModule.ApiKey))
                {
                    var arg = new SimilarArtistsArgument();
                    arg.Name = artistName;
                    arg.Results = 50;
                    arg.Bucket = ArtistBucket.Images | ArtistBucket.Terms;

                    var response = session
                        .Query<SimilarArtists>()
                        .Execute(arg);

                    if (response.Status.Code == ResponseCode.Success)
                    {
                        return response.Artists;
                    }
                }
            }

            return new ArtistBucketItem[0];
        }

        private void PresentSimilarArtists(IEnumerable<ArtistBucketItem> similarArtists)
        {
            using (LoadingIndicatorService.EnterLoadingBlock())
            {
                _similarArtists.Clear();

                foreach (var bucket in similarArtists)
                {
                    _similarArtists.Add(ConvertToModel(bucket));
                }
            }
        }

        #endregion Methods
    }
}