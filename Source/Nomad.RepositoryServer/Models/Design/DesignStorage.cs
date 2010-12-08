using System;
using System.Collections.Generic;
using Nomad.Modules.Manifest;
using Version = Nomad.Utils.Version;

namespace Nomad.RepositoryServer.Models.Design
{
    public class DesignStorage : IStorageProvider
    {
        public List<IModuleInfo> GetAvaliableModules()
        {
            var manifest = new ModuleManifest();
            manifest.Issuer = "TEST_1";
            manifest.ModuleName = "Module 1";
            manifest.ModuleVersion = new Version("1.0.0.0");

            var manifest2 = new ModuleManifest();
            manifest2.Issuer = "TEST_1";
            manifest2.ModuleName = "Module 1";
            manifest2.ModuleVersion = new Version("1.0.0.0");

            //TODO: implement dependencies and signed files

            return new List<IModuleInfo>()
                       {
                           new ModuleInfo(manifest,"example_path","#1"),
                           new ModuleInfo(manifest2,"example_path2","#2")
                       };
        }
    }
}