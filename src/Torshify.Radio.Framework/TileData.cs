using System;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Framework
{
    public class TileData : NotificationObject
    {
        #region Fields

        private Uri _backBackgroundImage;
        private object _backContent;
        private Uri _backgroundImage;
        private string _backTitle;
        private string _title;
        private bool _isLarge;

        #endregion Fields

        #region Properties

        public Uri BackBackgroundImage
        {
            get { return _backBackgroundImage; }
            set
            {
                if (_backBackgroundImage != value)
                {
                    _backBackgroundImage = value;
                    RaisePropertyChanged("BackBackgroundImage");
                }
            }
        }

        public object BackContent
        {
            get { return _backContent; }
            set
            {
                if (_backContent != value)
                {
                    _backContent = value;
                    RaisePropertyChanged("BackContent");
                }
            }
        }

        public Uri BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                if (_backgroundImage != value)
                {
                    _backgroundImage = value;
                    RaisePropertyChanged("BackgroundImage");
                }
            }
        }

        public string BackTitle
        {
            get { return _backTitle; }
            set
            {
                if (_backTitle != value)
                {
                    _backTitle = value;
                    RaisePropertyChanged("BackTitle");
                }
            }
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

        public bool IsLarge
        {
            get { return _isLarge; }
            set
            {
                if (_isLarge != value)
                {
                    _isLarge = value;
                    RaisePropertyChanged("IsLarge");
                }
            }
        }

        #endregion Properties
    }
}