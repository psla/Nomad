using System.Windows;
using System.Linq;

namespace Nomad.Regions.Behaviors
{
    public class ActiveAwareBehavior
    {
        public void Attach(IRegion region, DependencyObject regionHost)
        {
            region.ActiveViews.CollectionChanged +=
                (s, e) =>
                    {
                        if(e.NewItems != null)
                        foreach (var newItem in e.NewItems.OfType<IActiveAware>())
                        {
                            newItem.SetIsActive(true);
                        }

                        if (e.OldItems != null)
                        foreach (var oldItem in e.OldItems.OfType<IActiveAware>())
                        {
                            oldItem.SetIsActive(false);
                        }
                    };
        }
    }
}