using System.Linq;

namespace Nomad.Modules
{
    public class CompositeModuleFilter : IModuleFilter
    {
        private readonly IModuleFilter[] _moduleFilters;


        public CompositeModuleFilter(params IModuleFilter[] moduleFilters)
        {
            _moduleFilters = moduleFilters;
        }


        public bool Matches(ModuleInfo moduleInfo)
        {
            return _moduleFilters.All(filter => filter.Matches(moduleInfo));
        }
    }
}