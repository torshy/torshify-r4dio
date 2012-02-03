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

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.EchoNest.Views.Similar.Tabs
{
    [Export(typeof(SimilarViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
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

        public StaticCommand<SimilarArtistModel> PlayArtistCommand
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
            }
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext context)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext context)
        {
        }

        private void ExecutePlaySimilarArtist(SimilarArtistModel artist)
        {
            Radio.Play(new SimilarArtistsTrackStream(Radio, new[] { artist.BucketItem.Name })
                       {
                           Description = "Similar artists of " + _currentMainArtist
                       });
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
                                      PresentSimilarArists(task.Result);
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

        private void PresentSimilarArists(IEnumerable<ArtistBucketItem> similarArtists)
        {
            using (LoadingIndicatorService.EnterLoadingBlock())
            {
                _similarArtists.Clear();

                foreach (var bucket in similarArtists)
                {
                    _similarArtists.Add(new SimilarArtistModel
                                        {
                                            BucketItem = bucket,
                                            Name = bucket.Name,
                                            Image = bucket.Images != null ? bucket.Images.FirstOrDefault() : null,
                                            Terms = bucket.Terms != null ? bucket.Terms.Take(3) : null
                                        });
                }
            }
        }

        #endregion Methods
    }
}