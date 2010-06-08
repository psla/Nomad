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
        /// <summary>Gets type of control that this adapter can adapt to be region host.</summary>
        public Type SupportedType
        {
            get { return typeof (TabControl); }
        }


        /// <summary>
        ///     Creates and attaches new region to <paramref name="regionHost"/>
        /// </summary>
        /// <param name="regionHost">Control that will become a region host</param>
        /// <returns>Created and attached region</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="regionHost"/> is null</exception>
        /// <exception cref="InvalidOperationException">
        ///     When <paramref name="regionHost"/> is of type that is not supported by this adapter
        /// </exception>
        public IRegion AdaptView(DependencyObject regionHost)
        {
            if (regionHost == null) throw new ArgumentNullException("regionHost");
            if (!(regionHost is TabControl))
                throw new InvalidOperationException(
                    "This adapter only supports regionHosts that are TabControls");

            var tabControl = (TabControl) regionHost;
            var region = new SingleActiveViewRegion();

            new SynchronizeItemsBehavior().Attach(region, tabControl);
            new SynchronizeActiveItemsBehavior().Attach(region, tabControl);
            new ActiveAwareBehavior().Attach(region, tabControl);
            return region;
        }
    }
}