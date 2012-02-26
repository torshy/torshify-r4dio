using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Logging;

using Raven.Client;

using Torshify.Radio.Core.Models;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Views.Settings.General
{
    [Export("GeneralSettingsSection", typeof(ISettingsSection))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TrackSourceSection : ISettingsSection
    {
        #region Fields

        private readonly ObservableCollection<string> _trackSourcePriority;

        [Import]
        private IDocumentStore _documentStore = null;
        [Import]
        private ILoggerFacade _logger = null;
        [Import]
        private IToastService _toastService = null;
        [ImportMany]
        private IEnumerable<Lazy<ITrackSource, ITrackSourceMetadata>> _trackSources = null;

        #endregion Fields

        #region Constructors

        public TrackSourceSection()
        {
            HeaderInfo = new HeaderInfo
            {
                Title = "Track sources"
            };

            UI = new TrackSourceSectionView
            {
                DataContext = this
            };

            _trackSourcePriority = new ObservableCollection<string>();
        }

        #endregion Constructors

        #region Properties

        public HeaderInfo HeaderInfo
        {
            get;
            private set;
        }

        public object UI
        {
            get;
            private set;
        }

        public IEnumerable<string> TrackSourcePriority
        {
            get
            {
                return _trackSourcePriority;
            }
        }

        #endregion Properties

        #region Methods

        public void Load()
        {
            try
            {
                using (var session = _documentStore.OpenSession())
                {
                    var settings = session.Query<ApplicationSettings>().FirstOrDefault();

                    if (settings != null)
                    {
                        if (settings.TrackSourcePriority != null && settings.TrackSourcePriority.Any())
                        {
                            _trackSourcePriority.AddRange(settings.TrackSourcePriority);
                        }
                        else
                        {
                            foreach (var trackSource in _trackSources)
                            {
                                _trackSourcePriority.Add(trackSource.Metadata.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _toastService.Show(new ToastData
                {
                    Message = "Error while loading general settings"
                });

                _logger.Log(e.ToString(), Category.Exception, Priority.Medium);
            }
        }

        public void Save()
        {
            try
            {
                using (var session = _documentStore.OpenSession())
                {
                    var settings = session.Query<ApplicationSettings>().FirstOrDefault();

                    if (settings != null)
                    {
                        if (_trackSourcePriority.Any())
                        {
                            settings.TrackSourcePriority = _trackSourcePriority.ToList();
                            session.Store(settings);
                            session.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _toastService.Show(new ToastData
                {
                    Message = "Error while saving general settings"
                });

                _logger.Log(e.ToString(), Category.Exception, Priority.Medium);
            }
        }

        public void ChangeTrackSourcePriority(string draggingItem, string toItem)
        {
            _trackSourcePriority.Move(_trackSourcePriority.IndexOf(draggingItem), _trackSourcePriority.IndexOf(toItem));
        }

        #endregion Methods
    }
}