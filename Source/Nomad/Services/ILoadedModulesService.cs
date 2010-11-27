using System.Collections.Generic;
using Nomad.Modules;

namespace Nomad.Services
{
    /// <summary>
    /// Nomad service that allows modules to be aware of all modules loaded in the application
    /// </summary>
    public interface ILoadedModulesService
    {
        ///<summary>
        /// Provides information about currently loaded modules.
        ///</summary>
        ///<returns><see cref="IList{ModuleInfo}"/> containing moduleInfos of all loaded modules.</returns>
        IList<ModuleInfo> GetLoadedModules();
    }
}