using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using EchoNest;
using EchoNest.Artist;
using Microsoft.Isam.Esent.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Style
{
    public class StyleRadioStationViewModel : NotificationObject
    {
        #region Fields

        private readonly IRadioStationContext _context;
        private readonly IRadio _radio;

        private IEnumerable<TermModel> _currentTermList;
        private bool _isLoading;

        #endregion Fields

        #region Constructors

        public StyleRadioStationViewModel(IRadio radio, IRadioStationContext context)
        {
            AvailableTerms = new ObservableCollection<TermModel>();
            CreatePlaylistCommand = new DelegateCommand<IEnumerable>(ExecuteCreatePlaylist);

            _radio = radio;
            _context = context;
            
            IsLoading = true;

            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task<IEnumerable<TermModel>>.Factory
                .StartNew(GetAvailableStyles)
                .ContinueWith(t =>
                {
                    AvailableTerms.Clear();

                    foreach (var termModel in t.Result)
                    {
                        if (StyleRadioStation.StyleCloudData.ContainsKey(termModel.Name))
                        {
                            termModel.Count = StyleRadioStation.StyleCloudData[termModel.Name];
                        }

                        AvailableTerms.Add(termModel);
                    }

                    IsLoading = false;
                }, ui);
        }

        #endregion Constructors

        #region Properties

        public ObservableCollection<TermModel> AvailableTerms
        {
            get;
            private set;
        }

        public ICommand CreatePlaylistCommand
        {
            get;
            private set;
        }

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            private set
            {
                _isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        #endregion Properties

        #region Methods

        private void ExecuteCreatePlaylist(IEnumerable moods)
        {
            _currentTermList = moods.Cast<TermModel>();

            foreach (var termModel in _currentTermList)
            {
                if (StyleRadioStation.StyleCloudData.ContainsKey(termModel.Name))
                {
                    termModel.Count = termModel.Count + 1;
                    StyleRadioStation.StyleCloudData[termModel.Name] = termModel.Count;
                }
                else
                {
                    StyleRadioStation.StyleCloudData[termModel.Name] = 1;
                }
            }

            StyleRadioStation.StyleCloudData.Flush();

            var termEnumerator = new StylesToArtistEnumerator();
            termEnumerator.Initialize(_currentTermList, _radio);

            _context
                .SetTrackProvider(new TrackProvider(termEnumerator.DoIt))
                .ContinueWith(
                    t => _context.GoToTracks(),
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static IEnumerable<TermModel> GetAvailableStyles()
        {
            try
            {
                using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
                {
                    var response = session
                        .Query<ListTerms>()
                        .Execute();

                    if (response.Status.Code == ResponseCode.Success)
                    {
                        return response.Terms.Select(t => new TermModel { Name = t.Name });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new TermModel[0];
        }

        #endregion Methods
    }
}