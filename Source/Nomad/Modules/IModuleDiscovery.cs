using System.Collections.Generic;

namespace Nomad.Modules
{
    /// <summary>
    ///     Discovers all modules that should be loaded.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     Collection of modules returned by discovery is filtered
    ///     by a <see cref="IModuleFilter"/> associated with the 
    ///     <see cref="ModuleManager"/> prior to loading modules.
    /// 
    ///     Furthermore, modules can fail to load and other modules
    ///     will not be loaded because of unsatisfied dependencies.
    /// 
    ///     It is therefore advised not to think about returned collection 
    ///     as a final list of modules that will be loaded. However, 
    ///     module will never be loaded if it is not in the collection
    ///     returned by <see cref="GetModules"/>.
    ///     </para>
    /// 
    ///     <para>
    ///     If it is possible for the implementation to fail because of any
    ///     reason (resources unavailable, invalid configuration etc.), the
    ///     exceptions thrown in <see cref="GetModules"/> will not be
    ///     caught by <see cref="ModuleManager"/>. It is a desired behavior,
    ///     because failure to discover modules is an unrecoverable failure
    ///     of module loading.
    ///     </para>
    /// </remarks>
    public interface IModuleDiscovery
    {
        /// <summary>
        ///     Enumerates all modules that should be loaded.
        /// </summary>
        /// <returns>A collection of module manifests. Must not be <c>null</c></returns>
        IEnumerable<ModuleInfo> GetModules();
    }
}