using System;
using Nomad.Regions.Behaviors;

namespace Nomad.Regions
{
    /// <summary>
    ///     Defines common actions for all region implementations.
    /// </summary>
    public interface IRegion
    {
        /// <summary>
        ///     Gets collection of all views that are members of this region
        /// </summary>
        IViewCollection Views { get; }

        /// <summary>
        ///     Gets collection of all views that are active (e.g. selected by user, visible, ...)
        /// </summary>
        /// <remarks>
        ///     Specific interpretation of what active means depends on type of control, that
        ///     is region's host, and on region adapter used.
        /// </remarks>
        IViewCollection ActiveViews { get; }


        /// <summary>
        ///     Adds view to this region
        /// </summary>
        /// <remarks>
        /// <para>
        ///     Altough this method will accept any view, specific region implementation
        ///     may have some requirements on the view (e.g. view may have to implement some
        ///     interface or to be of some specific type). Refer to documentation of 
        ///     <see cref="IRegionAdapter"/> for your host to learn about those requirements.
        ///     If region's implementation has such requirements and <paramref name="view"/>
        ///     does not meet them, this method will throw an <see cref="InvalidOperationException"/>.
        /// </para>
        /// </remarks>
        /// <param name="view">View to be added</param>
        /// <exception cref="ArgumentNullException">When <paramref name="view"/> is null</exception>
        /// <exception cref="InvalidOperationException">
        ///     When <paramref name="view"/> does not meet specific region's requirements
        /// </exception>
        void AddView(object view);

        /// <summary>
        ///     Marks <paramref name="view"/> as active.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     Depending on specific implementation, activating view may or may not
        ///     deactivate or activate other views. Refer to documentation of 
        ///     <see cref="IRegionAdapter"/> for your host to learn about this behavior.
        /// </para>
        /// <para>
        ///     In most cases (depending on specific region adapter implementation), 
        ///     view can be notified when it is activated or deactivated.
        ///     See <see cref="IActiveAware"/> interface, <see cref="ActiveAwareBehavior"/> behavior,
        ///     and refer to <see cref="IRegionAdapter"/> for your host to learn whether this
        ///     behavior has been enabled.
        /// </para>
        /// </remarks>
        /// <param name="view">View to be activated</param>
        /// <exception cref="ArgumentNullException">When <paramref name="view"/> is null</exception>
        void Activate(object view);

        /// <summary>
        ///     Marks <paramref name="view"/> as not active
        /// </summary>
        /// <param name="view">View to be deactivated</param>
        /// <para>
        ///     In most cases (depending on specific region adapter implementation), 
        ///     view can be notified when it is activated or deactivated.
        ///     See <see cref="IActiveAware"/> interface, <see cref="ActiveAwareBehavior"/> behavior,
        ///     and refer to <see cref="IRegionAdapter"/> for your host to learn whether this
        ///     behavior has been enabled.
        /// </para>
        /// <exception cref="ArgumentNullException">When <paramref name="view"/> is null</exception>
        void Deactivate(object view);
    }
}