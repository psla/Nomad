using System.Collections.Generic;
using System.ServiceModel;
using Nomad.Modules.Manifest;

namespace Nomad.Updater.ModuleRepositories
{
    // NOTE: If you change the interface name "IModulesRepository" here, you must also update the reference to "IModulesRepository" in Web.config.
    /// <summary>
    /// <see cref="ServiceContractAttribute"/> for repository.
    /// </summary>
    /// <remarks>
    ///     
    /// </remarks>
    [ServiceContract]
    public interface IModulesRepository
    {
        /// <summary>
        ///     Gets all available modules.
        /// </summary>
        /// <returns>Complex class that represents the <see cref="List{T}"/> of <see cref="ModuleManifest"/> </returns>
        [OperationContract]
        AvailableModules GetAvailableModules();

        /// <summary>
        ///     Gets the single <see cref="ModulePackage"/> from server identified by module unique name.
        /// </summary>
        /// <param name="moduleUniqeName">Unique name of the module which will be got.</param>
        /// <returns>Module Package that contains the specified module</returns>
        [OperationContract]
        ModulePackage GetModule(string moduleUniqeName);
        // TODO: Add your service operations here

        /// <summary>
        /// Add modules to repository. 
        /// </summary>
        /// <param name="module">Unique module name of the new module.</param>
        /// <returns>True if addition was succesful.</returns>
        [OperationContract]
        bool AddModule(ModulePackage module);
    }
}
