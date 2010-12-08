using Nomad.Modules.Manifest;

namespace Nomad.RepositoryServer.Models
{
    public interface IModuleInfo
    {
        ModuleManifest Manifest { get; }
        string Id { get; }
        byte[] ModuleData { get; }
    }
}