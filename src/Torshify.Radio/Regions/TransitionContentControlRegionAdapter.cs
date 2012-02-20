using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Practices.Prism.Regions;
using Torshify.Radio.Framework.Controls;

namespace Torshify.Radio.Regions
{
    [Export]
    public class TransitionContentControlRegionAdapter : RegionAdapterBase<TransitioningContentControl>
    {
        [ImportingConstructor]
        public TransitionContentControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory)
        {

        }

        protected override void Adapt(IRegion region, TransitioningContentControl regionTarget)
        {
            if (regionTarget == null)
            {
                throw new ArgumentNullException("regionTarget");
            }
            if ((regionTarget.Content != null) || (BindingOperations.GetBinding(regionTarget, ContentControl.ContentProperty) != null))
            {
                throw new InvalidOperationException("ContentControlHasContentException");
            }
            region.ActiveViews.CollectionChanged += delegate
            {
                regionTarget.Content = region.ActiveViews.FirstOrDefault<object>();
            };
            region.Views.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
            {
                if ((e.Action == NotifyCollectionChangedAction.Add) && (region.ActiveViews.Count<object>() == 0))
                {
                    region.Activate(e.NewItems[0]);
                }
            };
        }

        protected override IRegion CreateRegion()
        {
            return new TransitionSingleActiveRegion();
        }

    }
}