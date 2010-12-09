using System.IO;
using Nomad.Modules.Manifest;

namespace Nomad.RepositoryServer.Models.DirectoryStorage
{
    public class DirectoryModuleInfo : IModuleInfo
    {
        private readonly string _packagePath;

        public DirectoryModuleInfo(ModuleManifest manifest, string path, string id)
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