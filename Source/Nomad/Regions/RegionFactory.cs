using System;
using System.Windows;
using System.Linq;

namespace Nomad.Regions
{
    /// <summary>
    ///     Default implementation of <see cref="IRegionFactory"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <see cref="RegionFactory"/> creates regions using a collection
    ///     of <see cref="IRegionAdapter"/>. Every <see cref="IRegionAdapter"/>
    ///     declares what type of views it can adapt. The <see cref="RegionFactory"/>
    ///     then uses the most appropriate <see cref="IRegionAdapter"/> to create
    ///     a region that will be attached to a view.
    /// </para>
    /// <para>
    ///     When looking for appropriate adapter, region factory begins by looking for adapter
    ///     that's supported type is type of view. If no such adapter is found, an adapter
    ///     supporting view's base type is used. Type hierarchy is then traversed up until
    ///     an adapter is found or all types have been considered. If no adapter is found by 
    ///     that time, an exception is thrown.
    /// </para>
    /// <para>
    ///     <see cref="IRegionAdapter">Region adapters</see> have to be known during creation
    ///     of the <see cref="RegionFactory">region factory</see>. At the moment, there is no
    ///     support for adding region adapters after the factory has been created.
    /// </para>
    /// </remarks>
    public class RegionFactory : IRegionFactory
    {
        private readonly IRegionAdapter[] _regionAdapters;


        /// <summary>
        /// Initializes new instance of <see cref="RegionFactory"/>.
        /// </summary>
        /// <param name="regionAdapters">A collection of region adapters that will be used by this <see cref="RegionFactory"/>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="regionAdapters"/> is null.</exception>
        public RegionFactory(IRegionAdapter[] regionAdapters)
        {
            if (regionAdapters == null) throw new ArgumentNullException("regionAdapters");

            _regionAdapters = regionAdapters.ToArray();
        }


        /// <summary>
        ///     Creates and attaches new region to the specified <paramref name="view"/>
        /// </summary>
        /// <param name="view">View to create new region for</param>
        /// <returns>Initialized and attached <see cref="IRegion"/> object</returns>
        /// <exception cref="InvalidOperationException">When a new region cannot be instantiated, beause view's type is not supported by this factory</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="view"/> is null</exception>
        public IRegion CreateRegion(DependencyObject view)
        {
            if (view == null) throw new ArgumentNullException("view");

            var adapter = FindAdapterFor(view);
            if (adapter == null)
                throw new InvalidOperationException("No adapter has been found for view of type " +
                                                    view.GetType().Name);

            return adapter.AdaptView(view);
        }


        private IRegionAdapter FindAdapterFor(DependencyObject view)
        {
            var type = view.GetType();
            while (type != null)
            {
                var adapter = FindAdapterForExactType(type);
                if (adapter != null)
                    return adapter;

                type = type.BaseType;
            }

            return null;
        }


        private IRegionAdapter FindAdapterForExactType(Type type)
        {
            return _regionAdapters.FirstOrDefault(adapter => adapter.SupportedType == type);
        }
    }
}