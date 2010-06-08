using System;
using System.Windows;

namespace Nomad.Regions
{
    /// <summary>
    ///     Region behavior is an object that ties region and it's host together.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Most interaction scenarios between region and it's host should
    ///     be achievable by combining multiple <see cref="IRegionBehavior"/> and
    ///     appropriate implementation of <see cref="IRegion"/>.
    /// </para>
    /// <para>
    ///     Concrete implementation of <see cref="IRegionBehavior"/> may choose
    ///     to add additional requirements to region or region host (e.g. support
    ///     only certain types).
    /// </para>
    /// </remarks>
    public interface IRegionBehavior
    {
        /// <summary>
        ///     Attaches all necesary event handlers, sets bindings an properties
        /// </summary>
        /// <param name="region">Region to attach to</param>
        /// <param name="regionHost">Region host to attach to</param>
        /// <exception cref="ArgumentNullException">
        ///     When <paramref name="region"/> or <paramref name="regionHost"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     When <paramref name="region"/> or <paramref name="regionHost"/> do not 
        ///     meet requirements of this <see cref="IRegionBehavior"/>
        /// </exception>
        void Attach(IRegion region, DependencyObject regionHost);
    }
}