using System;
using Nomad.Modules.Manifest;

namespace Nomad.Updater.ModuleRepositories.WebRepositories
{
    [Serializable]
    public class WebModulePackageInfo
    {
        public WebModulePackageInfo(ModuleManifest manifest, string url)
        {
            Manifest = manifest;
            Url = url;
        }

        /// <summary>
        ///     Parameterless constructor for XML serialization.
        /// </summary>
        public WebModulePackageInfo()
        {
            
        }

        public string Url { get;  set; }
        public ModuleManifest Manifest { get;  set; }
    }
}