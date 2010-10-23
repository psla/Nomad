namespace Nomad.Modules.Manifest
{
    ///<summary>
    /// Creates <see cref="ModuleManifest"/> based on provided <see cref="ModuleInfo"/>
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