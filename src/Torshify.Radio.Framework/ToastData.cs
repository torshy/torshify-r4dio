using System;

using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Framework
{
    public class ToastData : NotificationObject
    {
        #region Constructors

        public ToastData()
        {
            Icon = AppIcons.InformationWithCircle;
            DisplayTime = 2500;
        }

        #endregion Constructors

        #region Properties

        public Uri Icon
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public int DisplayTime
        {
            get; set;
        }

        #endregion Properties
    }
}