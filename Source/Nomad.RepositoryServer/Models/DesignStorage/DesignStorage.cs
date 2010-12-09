using System;
using System.Collections.Generic;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Models.DirectoryStorage;
using Version = Nomad.Utils.Version;

namespace Nomad.RepositoryServer.Models.DesignStorage
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
            manifest2.Issuer = "TEST_2";
            manifest2.ModuleName = "Module 2";
            manifest2.ModuleVersion = new Version("2.1.1.1");

            //TODO: implement dependencies and signed files

            return new List<IModuleInfo>()
                       {
                           new DirectoryModuleInfo(manifest,"example_path","#1"),
                           new DirectoryModuleInfo(manifest2,"example_path2","#2")
                       };
        }


        public bool SaveModule(IModuleInfo moduleInfo)
        {
            throw new NotImplementedException();
        }


        public bool RemoveModule(IModuleInfo moduleInfo)
        {
            throw new NotImplementedException();
        }
    }
}