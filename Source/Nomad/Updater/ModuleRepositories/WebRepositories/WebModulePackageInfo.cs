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


        public string Url { get; private set; }
        public ModuleManifest Manifest { get; private set; }
    }
}