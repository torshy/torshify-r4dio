using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.WindowsAPICodePack.Taskbar;

using Torshify.Radio.Framework;
using Torshify.Radio.Framework.Events;

using JumpList = System.Windows.Shell.JumpList;

namespace Torshify.Radio.Core.Startables
{
    public class TaskbarHandler : IStartable
    {
        #region Fields

        [Import]
        private IEventAggregator _eventAggregator = null;
        private JumpList _jumpList;
        [Import]
        private IRadio _radio = null;
        [Import]
        private IRegionManager _regionManager = null;
        [Import]
        private ITileService _tileService = null;

        #endregion Fields

        #region Methods

        public void Start()
        {
            if (TaskbarManager.IsPlatformSupported)
            {
                _eventAggregator.GetEvent<ApplicationArgumentsEvent>().Subscribe(OnArgumentsReceived, ThreadOption.UIThread, true);
                _jumpList = new JumpList();
                _tileService.Tiles.ForEach(AddJumpTask);
                _tileService.TileAdded += (sender, args) => AddJumpTask(args.Tile);
                JumpList.SetJumpList(Application.Current, _jumpList);
            }
        }

        private void OnArgumentsReceived(IEnumerable<string> args)
        {
            string query;

            if (args.Count() > 1)
            {
                query = args.Skip(1).FirstOrDefault();
            }
            else
            {
                query = args.FirstOrDefault();
            }

            UriQuery q = new UriQuery(query);

            if (!string.IsNullOrEmpty(q["ViewId"]) && !string.IsNullOrEmpty(q["RegionId"]))
            {
                _regionManager.RequestNavigate(q["RegionId"], q["ViewId"]);
            }
        }

        private void AddJumpTask(Tile tile)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action<Tile>(t =>
                {
                    UriQuery q = new UriQuery();
                    q.Add("ViewId", t.NavigationUri.OriginalString);
                    q.Add("RegionId", t.TargetRegionName);

                    JumpTask task = new JumpTask();
                    task.Title = t.Data.Title;
                    task.Arguments = q.ToString();

                    _jumpList.JumpItems.Add(task);
                    _jumpList.Apply();
                }),
                DispatcherPriority.ContextIdle,
                tile);
        }

        #endregion Methods
    }
}