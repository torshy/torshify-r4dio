using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Torshify.Radio.EchoNest.Views.Browse.Tabs;
using Torshify.Radio.EchoNest.Views.Similar;
using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Events;

namespace Torshify.Radio.EchoNest
{
    public class EchoNestEventsHandler : IStartable
    {
        #region Properties

        [Import]
        public IEventAggregator EventAggregator { get; set; }

        [Import]
        public IRegionManager RegionManager { get; set; }

        #endregion Properties

        #region Methods

        public void Start()
        {
            EventAggregator
                .GetEvent<BuildArtistRelatedCommandBarEvent>()
                .Subscribe(OnBuildArtistRelatedCommandBar, ThreadOption.PublisherThread);

            EventAggregator
                .GetEvent<BuildAlbumRelatedCommandBarEvent>()
                .Subscribe(OnBuildAlbumRelatedCommandBar, ThreadOption.PublisherThread);
        }

        private void OnBuildArtistRelatedCommandBar(ArtistRelatedCommandBarPayload payload)
        {
            payload.CommandBar.AddCommand(
                new CommandModel
                {
                    Content = "Browse",
                    Command = new DelegateCommand<string>(ExecuteBrowseForArtist),
                    CommandParameter = payload.ArtistName,
                    Icon = AppIcons.Search.ToImage()
                });

            payload.CommandBar.AddCommand(
                new CommandModel
                {
                    Content = "Find similar artists",
                    Command = new DelegateCommand<string>(ExecuteBrowseForSimilarArtists),
                    CommandParameter = payload.ArtistName
                });
        }

        private void OnBuildAlbumRelatedCommandBar(AlbumRelatedCommandBarPayload payload)
        {
            payload.CommandBar.AddCommand(
                new CommandModel
                {
                    Content = "Browse",
                    Command = new DelegateCommand<Tuple<string, string>>(ExecuteBrowseForAlbum),
                    CommandParameter = Tuple.Create(payload.ArtistName, payload.AlbumName),
                    Icon = AppIcons.Search.ToImage()
                });
        }

        private void ExecuteBrowseForArtist(string artistName)
        {
            if (!string.IsNullOrEmpty(artistName))
            {
                UriQuery query = new UriQuery();
                query.Add("artistName", artistName);
                RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof (ArtistView).FullName + query);
            }
        }

        private void ExecuteBrowseForSimilarArtists(string artistName)
        {
            if (!string.IsNullOrEmpty(artistName))
            {
                UriQuery query = new UriQuery();
                query.Add(SearchBar.IsFromSearchBarParameter, "true");
                query.Add(SearchBar.ValueParameter, artistName);
                RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof (MainStationView).FullName + query);
            }
        }

        private void ExecuteBrowseForAlbum(Tuple<string, string> parameter)
        {
            if (!string.IsNullOrEmpty(parameter.Item1) && !string.IsNullOrEmpty(parameter.Item2))
            {
                UriQuery query = new UriQuery();
                query.Add("artistName", parameter.Item1);
                query.Add("albumName", parameter.Item2);
                RegionManager.RequestNavigate(AppRegions.ViewRegion, typeof (AlbumView).FullName + query);
            }
        }

        #endregion Methods
    }
}