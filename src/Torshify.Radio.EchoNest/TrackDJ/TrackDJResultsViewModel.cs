using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using EchoNest;
using EchoNest.Song;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Common;
using System.Linq;

namespace Torshify.Radio.EchoNest.TrackDJ
{
    [Export(typeof(TrackDJResultsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TrackDJResultsViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IRadio _radio;

        private bool _isLoading;
        private ObservableCollection<RadioTrack> _tracks;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public TrackDJResultsViewModel(IRadio radio)
        {
            _radio = radio;
            _tracks = new ObservableCollection<RadioTrack>();
        }

        #endregion Constructors

        #region Properties

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    RaisePropertyChanged("IsLoading");
                }
            }
        }

        public ObservableCollection<RadioTrack> Results
        {
            get
            {
                return _tracks;
            }
        }

        #endregion Properties

        #region Methods

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var setup = navigationContext.NavigationService.Region.Context as TrackDJSetupViewModel;

            if (setup != null)
            {
                StartSearch(setup);
            }
        }

        private void StartSearch(TrackDJSetupViewModel setup)
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                List<RadioTrack> allTracks = new List<RadioTrack>();

                IsLoading = true;
                using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
                {
                    var arg = setup.CreateSearchArgument();
                    var result = session.Query<Search>().Execute(arg);

                    if (result != null && result.Status.Code == ResponseCode.Success)
                    {
                        var artists = result.Songs.GroupBy(s => s.ArtistName);

                        foreach (var artist in artists)
                        {
                            var tracks = _radio.GetTracksByArtist(artist.Key, 0, 100);

                            foreach (var songBucketItem in artist)
                            {
                                var track = tracks.FirstOrDefault(
                                    t =>
                                    t.Name.Equals(songBucketItem.Title, StringComparison.InvariantCultureIgnoreCase));

                                if (track != null)
                                {
                                    allTracks.Add(track);
                                }
                            }
                        }
                        foreach (var song in result.Songs)
                        {
                            var tracks = _radio.GetTracksByName(song.ArtistName + " " + song.Title, 0, 1);
                            allTracks.AddRange(tracks);
                        }
                    }
                }

                IsLoading = false;
                return allTracks;
            })
            .ContinueWith(t =>
            {
                _tracks.Clear();

                foreach (var radioTrack in t.Result)
                {
                    _tracks.Add(radioTrack);
                }
            }, ui);
        }

        #endregion Methods
    }
}