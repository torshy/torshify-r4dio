using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using EchoNest;
using EchoNest.Playlist;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using System.Linq;

namespace Torshify.Radio.EchoNest.Views.LoveHate
{
    [Export]
    [RegionMemberLifetime(KeepAlive = false)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoveHateViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private double? _currentTrackRating;

        #endregion Fields

        #region Properties

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
        public ILoadingIndicatorService LoadingIndicatorService
        {
            get;
            set;
        }

        [Import]
        public ISearchBarService SearchBarService
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

        public double? CurrentTrackRating
        {
            get
            {
                return _currentTrackRating;
            }
            set
            {
                if (_currentTrackRating != value)
                {
                    _currentTrackRating = value;
                    RaisePropertyChanged("CurrentTrackRating");
                }
            }
        }

        #endregion Properties

        #region Methods

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            SearchBarService.Add<LoveHateView>(new SearchBarData
            {
                Category = "Love_Or_Hate",
                WatermarkText = "Love_Or_Hate_Watermark"
            });

            SearchBarService.SetActive(searchBar => searchBar.NavigationUri.OriginalString.StartsWith(typeof(LoveHateView).FullName));

            if (!string.IsNullOrEmpty(navigationContext.Parameters[SearchBar.IsFromSearchBarParameter]))
            {
                var artistName = navigationContext.Parameters[SearchBar.ValueParameter];
                Execute(artistName);
            }
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            SearchBarService.Remove(
                searchBar => searchBar.NavigationUri.OriginalString.StartsWith(typeof(LoveHateView).FullName));
        }

        private void Execute(string artistName)
        {
            Task.Factory.StartNew(state =>
            {
                LoadingIndicatorService.Push();
                    using (var session = new EchoNestSession(EchoNestModule.ApiKey))
                    {
                        DynamicArgument argument = new DynamicArgument();
                        argument.Type = "artist-radio";
                        argument.Artist.Add(state.ToString());
                        return session.Query<Dynamic>().Execute(argument);
                    }
            },
            artistName)
            .ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    Logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                    ToastService.Show("Error while fetching playlist");
                }
                else
                {
                    if (task.Result.Status.Code == ResponseCode.Success)
                    {
                        var song = task.Result.Songs.FirstOrDefault();

                        if (song != null)
                        {
                            var tracks = Radio.GetTracksByName(song.ArtistName + " " + song.Title);

                            if (tracks.Any())
                            {
                                Radio.Play(tracks.ToTrackStream("Love/hate"));
                            }
                        }
                    }
                    else
                    {
                        ToastService.Show(task.Result.Status.Message);
                    }
                }

                LoadingIndicatorService.Pop();
            });
        }

        #endregion Methods
    }
}