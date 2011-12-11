using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

using Torshify.Radio.Framework.Controls;
using Torshify.Radio.Framework.Events;

namespace Torshify.Radio.Framework.Behaviors
{
    public class ArtistCommandsBehavior : Behavior<UIElement>
    {
        #region Fields

        public static readonly DependencyProperty ArtistNameProperty = 
            DependencyProperty.Register(
                "ArtistName", 
                typeof(string), 
                typeof(ArtistCommandsBehavior),
                new FrameworkPropertyMetadata((string)null));

        #endregion Fields

        #region Properties

        public string ArtistName
        {
            get { return (string)GetValue(ArtistNameProperty); }
            set { SetValue(ArtistNameProperty, value); }
        }

        #endregion Properties

        #region Methods

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseRightButtonUp += AssociatedObjectOnMouseRightButtonUp;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseRightButtonUp -= AssociatedObjectOnMouseRightButtonUp;
            base.OnDetaching();
        }

        private void AssociatedObjectOnMouseRightButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var element = sender as FrameworkElement;

            if (element != null)
            {
                var eventAggregator = ServiceLocator.Current.TryResolve<IEventAggregator>();
                ArtistCommandsPayload payload = new ArtistCommandsPayload(ArtistName);
                payload.CommandBar.AddSeparator(ArtistName);
                element.ContextMenu = new CommandBarContextMenu {DataContext = payload.CommandBar};
                eventAggregator.GetEvent<ArtistCommandsEvent>().Publish(payload);
            }
        }

        #endregion Methods
    }
}