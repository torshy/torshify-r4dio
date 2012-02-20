using System.Linq;
using Microsoft.Practices.Prism.Regions;

namespace Torshify.Radio.Regions
{
    public class TransitionSingleActiveRegion : Region
    {
        public override void Activate(object view)
        {
            object currentActiveView = ActiveViews.FirstOrDefault();

            base.Activate(view);

            if (currentActiveView != null && currentActiveView != view && Views.Contains(currentActiveView))
            {
                base.Deactivate(currentActiveView);
            }
        }
    }
}