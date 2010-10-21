using System.Linq;

namespace Nomad.Modules.Filters
{
    /// <summary>
    /// Basic composite pattern implementation for IModuleFilter interface,
    /// allows recurring ModuleFiltering.
    /// </summary>
    public class CompositeModuleFilter : IModuleFilter
    {
        private readonly IModuleFilter[] _moduleFilters;

        /// <summary>
        /// Initializes the instance of <see cref="CompositeModuleFilter"/> class applying the provided array of <see cref="IModuleFilter"/>
        /// </summary>
        /// <param name="moduleFilters">Array of <see cref="IModuleFilter"/> object to configure the filter.</param>
        public CompositeModuleFilter(params IModuleFilter[] moduleFilters)
        {
            _moduleFilters = moduleFilters;
        }

        /// <summary>
        /// <see cref="IModuleFilter"/> method implementation.
        /// </summary>
        /// <param name="moduleInfo"></param>
        /// <returns></returns>
        public bool Matches(ModuleInfo moduleInfo)
        {
            return _moduleFilters.All(filter => filter.Matches(moduleInfo));
        }
    }
}