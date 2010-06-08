using System;
using System.Windows;
using System.Windows.Controls;
using Nomad.Regions.Behaviors;

namespace Nomad.Regions.Adapters
{
    /// <summary>
    ///     Region adapter for <see cref="TabControl"/> region hosts.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Regions created using this this adapter are <see cref="SingleActiveViewRegion">SingleActiveViewRegions</see>.
    ///     This allows region to mimic the behavior of <see cref="TabControl"/>, which does not allow multiple tabs to be
    ///     selected.
    /// </para>
    /// <para>
    ///     Region's view collection and region host's items collection are synchronized by <see cref="SynchronizeItemsBehavior"/>.
    ///     Region's active views and host's selected item are synchronized using <see cref="SynchronizeActiveItemsBehavior"/>.
    ///     Active views can be notified when they are activated/deactivated if they implement <see cref="IActiveAware"/> interface
    ///     (see <see cref="ActiveAwareBehavior"/>)
    /// </para>
    /// </remarks>
    public class TabControlAdapter : IRegionAdapter
    {
        public Type SupportedType
        {
            get { return typeof (TabControl); }
        }


        public IRegion AdaptView(DependencyObject view)
        {
            var tabControl = view as TabControl;
            var region = new SingleActiveViewRegion();

            new SynchronizeItemsBehavior().Attach(region, tabControl);
            new SynchronizeActiveItemsBehavior().Attach(region, tabControl);
            new ActiveAwareBehavior().Attach(region, tabControl);
            return region;
        }
    }
}