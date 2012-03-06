using System.ComponentModel.Composition;
using System.Threading.Tasks;
using EchoNest;
using EchoNest.Artist;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Torshify.Radio.Framework;

namespace Torshify.Radio.EchoNest.Views.Hot
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RegionMemberLifetime(KeepAlive = false)]
    public class HotArtistsViewModel : INavigationAware
    {
        private readonly IRadio _radio;
        private readonly IToastService _toastService;
        private readonly ILoggerFacade _logger;

        [ImportingConstructor]
        public HotArtistsViewModel(
            IRadio radio, 
            IToastService toastService,
            ILoggerFacade logger)
        {
            _radio = radio;
            _toastService = toastService;
            _logger = logger;
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            _toastService.Show("Getting top hot songs");

            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory
                .StartNew(() => _radio.Play(new TopHotttTrackStream(_radio)))
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                    }

                    navigationContext.NavigationService.Journal.GoBack();
                }, ui);
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}