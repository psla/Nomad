using System;
using System.Collections.Specialized;
using System.Windows;
using System.Linq;

namespace Nomad.Regions.Behaviors
{
    /// <summary>
    ///     Behavior that allows views to receive notifications when they are activated 
    ///     or deactivated.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Only views that implement <see cref="IActiveAware"/> interface will receive
    ///     notification.
    /// </para>
    /// </remarks>
    public class ActiveAwareBehavior
    {
        public void Attach(IRegion region, DependencyObject regionHost)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (regionHost == null) throw new ArgumentNullException("regionHost");

            region.ActiveViews.CollectionChanged +=
                RegionActiveViewsChanged;
        }


        private static void RegionActiveViewsChanged(object s, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems.OfType<IActiveAware>())
                    newItem.SetIsActive(true);
            }

            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems.OfType<IActiveAware>())
                    oldItem.SetIsActive(false);
            }
        }
    }
}