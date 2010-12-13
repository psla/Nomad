using Nomad.Modules.Manifest;

namespace Nomad.RepositoryServer.Models
{
    /// <summary>
    ///     Describes the immutable object of the single <see cref="Nomad"/> module.
    /// </summary>
    public interface IModuleInfo
    {
        /// <summary>
        ///     Manifest of the package.
        /// </summary>
        ModuleManifest Manifest { get; }
        
        /// <summary>
        ///     Id for the package.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        ///     Package itself.
        /// </summary>
        byte[] ModuleData { get; }
    }
}