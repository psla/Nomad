using System.IO;
using Nomad.Modules.Manifest;

namespace Nomad.RepositoryServer.Models
{
    public class ModuleInfo : IModuleInfo
    {
        private readonly string _packagePath;


        public ModuleInfo(ModuleManifest manifest, string path, string id)
        {
            Manifest = manifest;
            _packagePath = path;
            Id = id;
        }

        #region IModuleInfo Members

        public ModuleManifest Manifest { get; private set; }

        public string Id { get; private set; }

        public byte[] ModuleData
        {
            get { return File.ReadAllBytes(_packagePath); }
        }

        #endregion
    }
}