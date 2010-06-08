using System;
using System.Windows;

namespace Nomad.Regions
{
    /// <summary>
    ///     Responsible for attaching new region to region host.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Specific implementations choose concrete type of region that will be created.
    ///     They also attach custom <see cref="IRegionBehavior">region behaviors</see>.
    ///     When implementing a custom <see cref="IRegionAdapter"/>, developer should ensure
    ///     that in order to fully detach region from region host, detaching the region
    ///     and it's behaviors is sufficient.
    /// </para>
    /// <para>
    ///     A region and region host are considered fully detached, when no binding, no event handler
    ///     tie any properties of region and region host. Also, all host's properties set when attaching
    ///     should be reset to their former values. If it's impossible, they should be reset to default value.
    /// </para>
    /// </remarks>
    public interface IRegionAdapter
    {
        /// <summary>Gets type of control that this adapter can adapt to be region host.</summary>
        Type SupportedType { get; }


        /// <summary>
        ///     Creates and attaches new region to <paramref name="regionHost"/>
        /// </summary>
        /// <param name="regionHost">Control that will become a region host</param>
        /// <returns>Created and attached region</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="regionHost"/> is null</exception>
        /// <exception cref="InvalidOperationException">
        ///     When <paramref name="regionHost"/> is of type that is not supported by this adapter
        /// </exception>
        IRegion AdaptView(DependencyObject regionHost);
    }
}