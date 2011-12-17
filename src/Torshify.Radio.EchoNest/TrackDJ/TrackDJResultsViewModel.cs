using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using EchoNest;
using EchoNest.Artist;
using EchoNest.Song;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using System.Linq;
using Search = EchoNest.Song.Search;

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
            _tracks.Clear();
            Task.Factory.StartNew(() =>
            {
                List<RadioTrack> allTracks = new List<RadioTrack>();

                IsLoading = true;

                try
                {
                    using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
                    {
                        var arg = setup.CreateSearchArgument();
                        arg.Bucket = SongBucket.ArtistLocation;

                        var result = session.Query<Search>().Execute(arg);

                        if (result == null)
                        {
                            result = session.Query<Search>().Execute(arg);
                        }

                        if (result != null && result.Status.Code == ResponseCode.Success)
                        {
                            var artists = result.Songs.GroupBy(s => s.ArtistName);

                            Parallel.ForEach(artists, artist =>
                            {
                                var tracks = _radio.GetTracksByArtist(artist.Key, 0, 100);

                                foreach (var songBucketItem in artist)
                                {
                                    var track = tracks.FirstOrDefault(
                                        t =>
                                        t.Name.Equals(songBucketItem.Title,
                                                    StringComparison.
                                                        InvariantCultureIgnoreCase));

                                    if (track != null)
                                    {
                                        track.ExtraData.Terms = null;

                                        Task
                                            .Factory
                                            .StartNew(FindArtistInformation,
                                                    Tuple.Create(track, songBucketItem));

                                        allTracks.Add(track);
                                        Application.Current.Dispatcher.BeginInvoke(
                                            new Action<RadioTrack>(_tracks.Add), track);
                                    }
                                }
                            });
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                IsLoading = false;
                return allTracks;
            });
        }

        private void FindArtistInformation(object obj)
        {
            Tuple<RadioTrack, SongBucketItem> tuple = (Tuple<RadioTrack, SongBucketItem>)obj;
            RadioTrack radioTrack = tuple.Item1;
            SongBucketItem songItem = tuple.Item2;
            
            try
            {
                using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
                {
                    var response = session.Query<Profile>().Execute(new IdSpace(songItem.ArtistID), ArtistBucket.Terms);

                    if (response != null && response.Status.Code == ResponseCode.Success)
                    {
                        radioTrack.ExtraData.Terms = response.Artist.Terms.Take(3);
                    }
                }
            }
            catch(Exception ex)
            {
                
            }
        }

        #endregion Methods
    }
}