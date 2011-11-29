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
using Torshify.Radio.EchoNest.Mood;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Similar
{
    public class SimilarArtistsRadioStationViewModel : NotificationObject
    {
        #region Fields

        private readonly IRadio _radio;
        private readonly IRadioStationContext _context;

        private ObservableCollection<string> _autoCompleteSuggestions;
        private ObservableCollection<ArtistBucketItem> _similarArtists;

        #endregion Fields

        #region Constructors

        public SimilarArtistsRadioStationViewModel(IRadio radio, IRadioStationContext context)
        {
            _radio = radio;
            _context = context;
            _autoCompleteSuggestions = new ObservableCollection<string>();
            _similarArtists = new ObservableCollection<ArtistBucketItem>();

            SearchCommand = new DelegateCommand<string>(ExecuteSearch);
            CreatePlaylistCommand = new DelegateCommand(ExecuteCreatePlaylist);
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

        public ICommand SearchCommand
        {
            get;
            private set;
        }

        public IEnumerable<ArtistBucketItem> SimilarArtists
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

        private void ExecuteCreatePlaylist()
        {
            var termEnumerator = new ArtistNameToArtistEnumerator();
            termEnumerator.Initialize(SimilarArtists, _radio);

            _context
                .SetTrackProvider(termEnumerator.DoIt).
                ContinueWith(t => _context.GoToTracks(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ExecuteSearch(string searchPhrase)
        {
            Task<IEnumerable<ArtistBucketItem>>.Factory
                .StartNew(() =>
                {
                    using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
                    {
                        SimilarArtistsArgument arg = new SimilarArtistsArgument();
                        arg.Name = searchPhrase;

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
                    foreach (var name in t.Result)
                    {
                        _similarArtists.Add(name);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion Methods
    }
}