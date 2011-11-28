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
        private ObservableCollection<IRadioTrack> _results;

        #endregion Fields

        #region Constructors

        public SearchRadioStationViewModel(IRadio radio, IRadioStationContext context)
        {
            _radio = radio;
            _context = context;
            _results = new ObservableCollection<IRadioTrack>();

            SearchCommand = new DelegateCommand<string>(ExecuteSearch);
        }

        #endregion Constructors

        #region Properties

        public ICommand SearchCommand
        {
            get;
            private set;
        }

        public IEnumerable<IRadioTrack> Results
        {
            get
            {
                return _results;
            }
        }

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
        #endregion Properties

        #region Methods

        private void ExecuteSearch(string query)
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory
                .StartNew(() => _radio.GetTracksByName(query, 0, 10))
                .ContinueWith(t=>
                                  {
                                      _results.Clear();
                                      foreach (var radioTrack in t.Result)
                                      {
                                          _results.Add(radioTrack);
                                      }
                                  }, ui);
        }

        #endregion Methods
    }
}