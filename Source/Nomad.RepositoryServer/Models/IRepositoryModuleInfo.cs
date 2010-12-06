using Nomad.Modules.Manifest;

namespace Nomad.RepositoryServer.Models
{
    public interface IRepositoryModuleInfo
    {
        ModuleManifest Manifest { get; }
        string Url { get; }
        byte[] ModuleData { get; }
    }
}