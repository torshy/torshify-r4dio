using System;
using System.ComponentModel.Composition;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Regions;

using Torshify.Radio.Core.Views.Notifications;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Services
{
    [Export(typeof(IToastService))]
    public class ToastService : IToastService, IPartImportsSatisfiedNotification
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly ToastServiceView _view;
        private readonly ToastServiceViewModel _viewModel;
        private IRegion _region;

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public ToastService(IRegionManager regionManager, ToastServiceView view)
        {
            _regionManager = regionManager;
            _view = view;
            _viewModel = view.Model;
            _viewModel.Activate += ViewModelOnActivate;
            _viewModel.Deactivate += ViewModelOnDeactivate;
        }

        #endregion Constructors

        #region Methods

        public void Show(string message, int displayTimeMs = 2500)
        {
            Show(new ToastData { Message = message, DisplayTime = displayTimeMs });
        }

        public void Show(ToastData data)
        {
            if (_view.CheckAccess())
            {
                _viewModel.NewNotification(data);
            }
            else
            {
                _view.Dispatcher.BeginInvoke(new Action<ToastData>(Show), DispatcherPriority.Background, data);
            }
        }

        public void OnImportsSatisfied()
        {
            _region = _regionManager.Regions[AppRegions.MainOverlayRegion];
        }

        private void ViewModelOnDeactivate(object sender, EventArgs eventArgs)
        {
            if (_view.CheckAccess())
            {
                if (_region.Views.Contains(_view))
                {
                    _region.Remove(_view);
                }
            }
            else
            {
                _view.Dispatcher.BeginInvoke(new Action<object, EventArgs>(ViewModelOnDeactivate), sender, eventArgs);
            }
        }

        private void ViewModelOnActivate(object sender, EventArgs eventArgs)
        {
            if (_view.CheckAccess())
            {
                if (!_region.Views.Contains(_view))
                {
                    _region.Add(_view);
                }
            }
            else
            {
                _view.Dispatcher.BeginInvoke(new Action<object, EventArgs>(ViewModelOnActivate), sender, eventArgs);
            }
        }

        #endregion Methods
    }
}