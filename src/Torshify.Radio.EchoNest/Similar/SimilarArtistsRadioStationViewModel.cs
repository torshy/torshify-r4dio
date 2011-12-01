using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using EchoNest;
using EchoNest.Artist;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Commands;

namespace Torshify.Radio.EchoNest.Similar
{
    public class SimilarArtistsRadioStationViewModel : NotificationObject
    {
        #region Fields

        private readonly IRadioStationContext _context;
        private readonly IRadio _radio;

        private ObservableCollection<string> _autoCompleteSuggestions;
        private bool _isLoading;
        private ObservableCollection<SimilarArtistModel> _similarArtists;
        private string _searchText;

        #endregion Fields

        #region Constructors

        public SimilarArtistsRadioStationViewModel(IRadio radio, IRadioStationContext context)
        {
            _radio = radio;
            _context = context;
            _autoCompleteSuggestions = new ObservableCollection<string>();
            _similarArtists = new ObservableCollection<SimilarArtistModel>();

            SearchCommand = new DelegateCommand<string>(ExecuteSearch);
            CreatePlaylistCommand = new AutomaticCommand(ExecuteCreatePlaylist, CanExecuteCreatePlaylist);
            PlayCommand = new DelegateCommand<SimilarArtistModel>(ExecutePlay);

            if (_radio.CurrentTrack != null)
            {
                SearchText = _radio.CurrentTrack.Artist;
            }
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<string> AutoCompleteSuggestions
        {
            get
            {
                return _autoCompleteSuggestions;
            }
        }

        public ICommand CreatePlaylistCommand
        {
            get;
            private set;
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                _isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        public ICommand PlayCommand
        {
            get;
            private set;
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    RaisePropertyChanged("SearchText");
                }
            }
        }

        public ICommand SearchCommand
        {
            get;
            private set;
        }

        public IEnumerable<SimilarArtistModel> SimilarArtists
        {
            get
            {
                return _similarArtists;
            }
        }

        #endregion Properties

        #region Methods

        public void UpdateAutoComplete(string searchText)
        {
            _autoCompleteSuggestions.Clear();

            Task.Factory
                .StartNew(() =>
                {
                    try
                    {
                        using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
                        {
                            var response = session
                                .Query<SuggestArtist>()
                                .Execute(searchText);

                            if (response.Status.Code == ResponseCode.Success)
                            {
                                return response.Artists.Select(t => t.Name);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    return new string[0];
                })
                .ContinueWith(t =>
                {
                    foreach (var name in t.Result)
                    {
                        _autoCompleteSuggestions.Add(name);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private bool CanExecuteCreatePlaylist()
        {
            return SimilarArtists.Any();
        }

        private void ExecuteCreatePlaylist()
        {
            var enumerator = new ArtistNameToArtistEnumerator();
            enumerator.Initialize(SimilarArtists.Select(s => s.BucketItem).ToArray(), _radio);

            _context
                .SetTrackProvider(enumerator.DoIt)
                .ContinueWith(t => _context.GoToTracks(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ExecutePlay(SimilarArtistModel artist)
        {
            var enumerator = new ArtistNameToArtistEnumerator();
            enumerator.NumberOfTracksPerArtist = 200;
            enumerator.Initialize(new[] { artist.BucketItem }, _radio);

            _context
                .SetTrackProvider(enumerator.DoIt)
                .ContinueWith(t => _context.GoToTracks(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ExecuteSearch(string searchPhrase)
        {
            Task<IEnumerable<ArtistBucketItem>>.Factory
                .StartNew(() =>
                              {
                                  IsLoading = true;
                                  using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
                                  {
                                      SimilarArtistsArgument arg = new SimilarArtistsArgument();
                                      arg.Name = searchPhrase;
                                      arg.Results = 20;
                                      arg.Bucket = ArtistBucket.Images | ArtistBucket.Terms;

                                      var response = session
                                          .Query<SimilarArtists>()
                                          .Execute(arg);

                                      if (response.Status.Code == ResponseCode.Success)
                                      {
                                          return response.Artists;
                                      }
                                  }

                                  return new ArtistBucketItem[0];
                              })
                .ContinueWith(t =>
                {
                    _similarArtists.Clear();

                    foreach (var name in t.Result)
                    {
                        _similarArtists.Add(new SimilarArtistModel
                                                {
                                                    BucketItem = name,
                                                    Name = name.Name,
                                                    Image = name.Images != null ? name.Images.FirstOrDefault() : null,
                                                    Terms = name.Terms != null ? name.Terms.Take(3) : null
                                                });
                    }

                    IsLoading = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion Methods
    }
}