using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using EightTracks;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;

namespace Torshify.Radio.EightTracks.Views.Tabs
{
    public abstract class MixListViewModel : NotificationObject
    {
        #region Fields

        private ObservableCollection<Mix> _mixes;

        #endregion Fields

        #region Constructors

        protected MixListViewModel()
        {
            _mixes = new ObservableCollection<Mix>();
        }

        #endregion Constructors

        #region Properties

        public abstract HeaderInfo HeaderInfo
        {
            get;
        }

        [Import]
        public ILoadingIndicatorService LoadingIndicatorService
        {
            get;
            set;
        }

        public IEnumerable<Mix> Mixes
        {
            get { return _mixes; }
        }

        #endregion Properties

        #region Methods

        protected virtual void SearchForMixes(Mixes.Sort sortType)
        {
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                using (LoadingIndicatorService.EnterLoadingBlock())
                {
                    using (var session = new EightTracksSession(EightTracksModule.ApiKey))
                    {
                        var response = session.Query<Mixes>().GetMix(sorting: sortType);
                        return response.Mixes;
                    }
                }
            })
            .ContinueWith(t =>
            {
                _mixes.Clear();

                foreach (var mix in t.Result)
                {
                    _mixes.Add(mix);
                }
            }, ui);
        }

        #endregion Methods
    }
}