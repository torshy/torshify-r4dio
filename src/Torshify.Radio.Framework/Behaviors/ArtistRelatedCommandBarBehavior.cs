using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

using Torshify.Radio.Framework.Controls;
using Torshify.Radio.Framework.Events;

namespace Torshify.Radio.Framework.Behaviors
{
    public class ArtistRelatedCommandBarBehavior : Behavior<UIElement>
    {
        #region Fields

        public static readonly DependencyProperty ArtistNameProperty = 
            DependencyProperty.Register(
                "ArtistName", 
                typeof(string), 
                typeof(ArtistRelatedCommandBarBehavior),
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
                var commandBar = new CommandBar();
                var payload = new ArtistRelatedCommandBarPayload(ArtistName, commandBar);
                element.ContextMenu = new CommandBarContextMenu {DataContext = commandBar};
                eventAggregator.GetEvent<BuildArtistRelatedCommandBarEvent>().Publish(payload);
            }
        }

        #endregion Methods
    }
}