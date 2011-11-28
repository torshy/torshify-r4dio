using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using EchoNest;
using EchoNest.Artist;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest
{
    public class AutoCompleteViewModel : NotificationObject
    {
        #region Fields

        private ObservableCollection<string> _autoCompleteSuggestions;

        #endregion Fields

        #region Constructors

        public AutoCompleteViewModel()
        {
            _autoCompleteSuggestions = new ObservableCollection<string>();
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

        #endregion Properties

        #region Methods

        public void UpdateAutoComplete(string searchText)
        {
            _autoCompleteSuggestions.Clear();

            Task.Factory
                .StartNew(() =>
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

        #endregion Methods
    }
}