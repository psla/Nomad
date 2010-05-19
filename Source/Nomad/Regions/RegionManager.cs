using System;
using System.Collections.Generic;
using System.Windows;

namespace Nomad.Regions
{
    /// <summary>
    ///     Default implementation of IRegionManager (TODO: Extract interface + reference it).
    /// </summary>
    public class RegionManager
    {
        private readonly IRegionFactory _regionFactory;
        private readonly Dictionary<string, IRegion> _regions = new Dictionary<string, IRegion>();

        /// <summary>
        ///     Initializes new instance of the <see cref="RegionManager"/> class.
        /// </summary>
        /// <param name="regionFactory"><see cref="IRegionFactory"/> that will be used to create new regions</param>
        /// <exception cref="ArgumentNullException">When any of dependencies is null</exception>
        public RegionManager(IRegionFactory regionFactory)
        {
            if (regionFactory == null) throw new ArgumentNullException("regionFactory");
            _regionFactory = regionFactory;
        }

        /// <summary>
        ///     Attaches new region to <paramref name="view"/> and registers it under <paramref name="regionName"/> name.
        /// </summary>
        /// <remarks>
        ///     Exact type of <see cref="IRegion"/> created and attached will be determined by <see cref="IRegionFactory"/>
        ///     used by this <see cref="RegionManager"/>. Decision will be made based on <paramref name="view"/> type, 
        ///     attributes or properties.
        /// </remarks>
        /// <param name="regionName">Name that new region will be registered under</param>
        /// <param name="view">View to attach region to</param>
        /// <returns>Attached region</returns>
        /// <exception cref="ArgumentException">
        ///     When another region has already been registered under <paramref name="regionName"/> name
        /// </exception>
        public IRegion AttachRegion(string regionName, DependencyObject view)
        {
            if (ContainsRegion(regionName))
                throw new ArgumentException(string.Format("Region \"{0}\" already exists", regionName));

            var region = _regionFactory.CreateRegion(view);

            _regions.Add(regionName, region);
            return region;
        }

        /// <summary>
        ///     Gets <see cref="IRegion"/> registered under <paramref name="regionName"/> name.
        /// </summary>
        /// <param name="regionName">Name of region to retrieve</param>
        /// <returns><see cref="IRegion"/> that was registered under <paramref name="regionName"/> name</returns>
        /// <exception cref="KeyNotFoundException">
        ///     When no region has been registered under requested <paramref name="regionName"/>
        /// </exception>
        public IRegion GetRegion(string regionName)
        {
            IRegion region;
            if (!_regions.TryGetValue(regionName, out region))
                throw new KeyNotFoundException(string.Format("Region \"{0}\" has not been attached", regionName));
            return region;
        }

        /// <summary>
        ///     Checks whether this <see cref="RegionManager"/> contains region named <paramref name="regionName"/>.
        /// </summary>
        /// <param name="regionName">Name of region to check</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="RegionManager"/> contains region registered under <paramref name="regionName"/>, 
        ///     <c>false</c> 
        /// </returns>
        public bool ContainsRegion(string regionName)
        {
            return _regions.ContainsKey(regionName);
        }
    }
}