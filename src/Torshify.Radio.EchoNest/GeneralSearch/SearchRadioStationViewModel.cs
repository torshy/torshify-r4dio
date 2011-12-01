using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Practices.Prism.Commands;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.GeneralSearch
{
    public class SearchRadioStationViewModel : AutoCompleteViewModel
    {
        #region Fields

        private readonly IRadioStationContext _context;
        private readonly IRadio _radio;

        private ObservableCollection<RadioTrack> _results;
        private bool _isLoading;

        #endregion Fields

        #region Constructors

        public SearchRadioStationViewModel(IRadio radio, IRadioStationContext context)
        {
            _radio = radio;
            _context = context;
            _results = new ObservableCollection<RadioTrack>();

            SearchCommand = new DelegateCommand<string>(ExecuteSearch);
            PlayCommand = new DelegateCommand<RadioTrack>(ExecutePlay);
        }

        #endregion Constructors

        #region Properties

        public ImageSource DefaultImage
        {
            get
            {
                var coverArtSource = new BitmapImage();
                coverArtSource.BeginInit();
                coverArtSource.CacheOption = BitmapCacheOption.None;
                coverArtSource.UriSource = new Uri("pack://application:,,,/Torshify.Radio;component/Resources/SmallIcons/play.png", UriKind.Absolute);
                coverArtSource.EndInit();

                if (coverArtSource.CanFreeze)
                {
                    coverArtSource.Freeze();
                }

                return coverArtSource;
            }
        }

        public IEnumerable<RadioTrack> Results
        {
            get
            {
                return _results;
            }
        }

        public ICommand SearchCommand
        {
            get;
            private set;
        }

        public ICommand PlayCommand
        {
            get; private set;
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

        #endregion Properties

        #region Methods

        private void ExecutePlay(RadioTrack track)
        {
            _context.SetTrackProvider(() => new[] {track});
        }

        private void ExecuteSearch(string query)
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory
                .StartNew(() =>
                              {
                                  IsLoading = true;
                                  return _radio.GetTracksByName(query, 0, 32);
                              })
                .ContinueWith(t=>
                                  {
                                      _results.Clear();
                                      foreach (var radioTrack in t.Result)
                                      {
                                          _results.Add(radioTrack);
                                      }

                                      IsLoading = false;
                                  }, ui);
        }

        #endregion Methods
    }
}