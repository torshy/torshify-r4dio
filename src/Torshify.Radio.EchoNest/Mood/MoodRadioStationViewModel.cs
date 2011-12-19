using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using EchoNest;
using EchoNest.Artist;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Mood
{
    public class MoodRadioStationViewModel : NotificationObject
    {
        #region Fields

        private readonly IRadioStationContext _context;
        private readonly IRadio _radio;

        private IEnumerable<TermModel> _currentMoodList;
        private bool _isLoading;

        #endregion Fields

        #region Constructors

        public MoodRadioStationViewModel(IRadio radio, IRadioStationContext context)
        {
            AvailableTerms = new ObservableCollection<TermModel>();
            CreatePlaylistCommand = new DelegateCommand<IEnumerable>(ExecuteCreatePlaylist);

            _radio = radio;
            _context = context;

            IsLoading = true;

            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task<IEnumerable<TermModel>>.Factory
                .StartNew(GetAvailableMoods)
                .ContinueWith(t =>
                {
                    AvailableTerms.Clear();

                    foreach (var termModel in t.Result)
                    {
                        if (MoodRadioStation.MoodCloudData.ContainsKey(termModel.Name))
                        {
                            termModel.Count = MoodRadioStation.MoodCloudData[termModel.Name];
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
            _currentMoodList = moods.Cast<TermModel>();

            foreach (var termModel in _currentMoodList)
            {
                if (MoodRadioStation.MoodCloudData.ContainsKey(termModel.Name))
                {
                    termModel.Count = termModel.Count + 1;
                    MoodRadioStation.MoodCloudData[termModel.Name] = termModel.Count;
                }
                else
                {
                    MoodRadioStation.MoodCloudData[termModel.Name] = 1;
                }
            }

            MoodRadioStation.MoodCloudData.Flush();

            _context
                .SetTrackProvider(new CustomTrackProvider(_currentMoodList.ToArray(), _radio))
                .ContinueWith(
                    t => _context.GoToTracks(),
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static IEnumerable<TermModel> GetAvailableMoods()
        {
            try
            {
                using (EchoNestSession session = new EchoNestSession(EchoNestConstants.ApiKey))
                {
                    var response = session
                        .Query<ListTerms>()
                        .Execute(ListTermsType.Mood);

                    if (response.Status.Code == ResponseCode.Success)
                    {
                        return response.Terms.Select(t => new TermModel {Name = t.Name});
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

        public class CustomTrackProvider : TrackProvider
        {
            #region Fields

            private MoodsToArtistEnumerator _enumerator;

            #endregion Fields

            #region Constructors

            public CustomTrackProvider(IEnumerable<TermModel> terms, IRadio radio)
            {
                _enumerator = new MoodsToArtistEnumerator();
                _enumerator.Initialize(terms, radio);

                BatchProvider = _enumerator.DoIt;
            }

            #endregion Constructors
        }

    }
}