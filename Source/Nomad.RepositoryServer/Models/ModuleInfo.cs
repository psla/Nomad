using Nomad.Modules.Manifest;

namespace Nomad.RepositoryServer.Models
{
    public class ModuleInfo : IModuleInfo
    {
        public ModuleManifest Manifest { get; set; }

        public string Id { get; set; }

        public byte[] ModuleData { get; set; }
    }
}