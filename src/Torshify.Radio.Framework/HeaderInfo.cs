using System;

using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Framework
{
    public class HeaderInfo : NotificationObject
    {
        #region Fields

        private string _iconUri;
        private bool _isSelected;
        private string _title;
        private object _toolTip;

        #endregion Fields

        #region Properties

        public string IconUri
        {
            get { return _iconUri; }
            set
            {
                if (_iconUri != value)
                {
                    _iconUri = value;
                    RaisePropertyChanged("IconUri");
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");

                    if (IsSelectedAction != null)
                    {
                        IsSelectedAction(value);
                    }
                }
            }
        }

        public Action<bool> IsSelectedAction
        {
            get;
            set;
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        public object ToolTip
        {
            get { return _toolTip; }
            set
            {
                if (_toolTip != value)
                {
                    _toolTip = value;
                    RaisePropertyChanged("ToolTip");
                }
            }
        }

        #endregion Properties
    }
}