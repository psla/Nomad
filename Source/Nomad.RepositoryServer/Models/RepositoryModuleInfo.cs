using System.IO;
using Nomad.Modules.Manifest;

namespace Nomad.RepositoryServer.Models
{
    public class RepositoryModuleInfo : IRepositoryModuleInfo
    {
        private readonly string _packagePath;


        public RepositoryModuleInfo(ModuleManifest manifest, string path)
        {
            Manifest = manifest;
            _packagePath = path;
        }

        #region IRepositoryModuleInfo Members

        public ModuleManifest Manifest { get; private set; }

        public string Url
        {
            get { return Manifest.ModuleName + '/' + Manifest.ModuleVersion + "/getPack"; }
        }

        public byte[] ModuleData
        {
            get { return File.ReadAllBytes(_packagePath); }
        }

        #endregion
    }
}