using System;
using Nomad.Modules.Manifest;

namespace Nomad.RepositoryServer.Models
{
    public class VirtualModuleInfo : IModuleInfo
    {
        public ModuleManifest Manifest { get; set; }

        public string Id { get; set; }

        public byte[] ModuleData { get; set; }
    }
}