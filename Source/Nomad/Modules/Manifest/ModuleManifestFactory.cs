using System;
using System.IO;
using Nomad.Utils;

namespace Nomad.Modules.Manifest
{
    ///<summary>
    /// Returns module manifest from real directory
    ///</summary>
    public class ModuleManifestFactory : IModuleManifestFactory
    {
        /// <summary>
        /// Inherited.
        /// </summary>
        public ModuleManifest GetManifest(ModuleInfo moduleInfo)
        {
            var manifestPath = string.Format("{0}{1}", moduleInfo.AssemblyPath,
                                             ModuleManifest.ManifestFileNameSuffix);
            var manifest = File.ReadAllBytes(manifestPath);
            return
                XmlSerializerHelper.Deserialize<ModuleManifest>(manifest);
        }
    }
}