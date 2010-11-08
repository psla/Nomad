using System;

namespace Nomad.Modules.Manifest
{
    ///<summary>
    ///     Creates <see cref="ModuleManifest"/> based on provided <see cref="ModuleInfo"/>. Performs as link layer between physical 
    /// location of manifest in file system and abstract program representation.
    ///</summary>
    public interface IModuleManifestFactory
    {
        ///<summary>
        /// Gets manifest for specified module
        ///</summary>
        ///<param name="moduleInfo">module info to get manifest for</param>
        ///<returns><see cref="ModuleManifest"/> for <see cref="ModuleInfo"/></returns>
        ModuleManifest GetManifest(ModuleInfo moduleInfo);

    }
}