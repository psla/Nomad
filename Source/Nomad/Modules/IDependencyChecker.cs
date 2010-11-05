using System.Collections.Generic;

namespace Nomad.Modules
{
    /// <summary>
    ///     Provides means for checking the dependencies between modules. 
    /// </summary>
    public interface IDependencyChecker
    {        

        /// <summary>
        ///     Sorts the given assemblies (modules) set in a dependency resolved loading order.
        /// </summary>
        /// <param name="modules">Assemblies set of modules to be sorted.</param>
        /// <returns>Properly ordered set of modules for module loading purpose.</returns>
        IEnumerable<ModuleInfo> SortModules(IEnumerable<ModuleInfo> modules);

        /// <summary>
        ///     Checks whether the given sets of modules can be merged with all dependencies resolved.
        ///     On failure populates the nonValidModules set with modules that don't have their dependencies resolved.
        /// </summary>
        /// <param name="loadedModules">Set of modules available in the application.</param>
        /// <param name="newModules"> List of modules to be added into the application or replace loaded modules.</param>
        /// <param name="nonValidModules">List of modules that cannot be loaded.</param>
        /// <returns>True on successful dependency resolution.</returns>
        bool CheckModules(IEnumerable<ModuleInfo> loadedModules, IEnumerable<ModuleInfo> newModules,
                          out IEnumerable<ModuleInfo> nonValidModules);
    }
}