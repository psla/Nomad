using System;
using System.Collections.Generic;
using System.IO;
using Nomad.Modules.Manifest;
using Nomad.Updater;
using Nomad.Updater.ModuleRepositories;
using Nomad.Utils;

namespace Nomad.ModulesRepository
{
    // NOTE: If you change the class name "ModulesRepository" here, you must also update the reference to "ModulesRepository" in Web.config and in the associated .svc file.
    public class ModulesRepository : IModulesRepository
    {
        private readonly string _modulesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules");
        public string ModulesDir { get { return _modulesDir; } }

        public ModulesRepository()
        {
            if (!Directory.Exists(_modulesDir))
                Directory.CreateDirectory(_modulesDir);
        }
        public AvailableModules GetAvailableModules()
        {
            var manifestsFiles = Directory.GetFiles(_modulesDir, string.Format("*{0}", ModuleManifest.ManifestFileNameSuffix));
            var manifestFactory = new ModuleManifestFactory();
            List<ModuleManifest> manifests = new List<ModuleManifest>();
            foreach (var manifestFile in manifestsFiles)
            {
                try
                {
                    var manifest = XmlSerializerHelper.Deserialize<ModuleManifest>(File.ReadAllBytes(manifestFile));
                    manifests.Add(manifest);
                }
                catch
                {
                    //TODO: LOGGING FAILURES
                }
            }
            return new AvailableModules(manifests);
        }


        public ModulePackage GetModule(string moduleUniqeName)
        {
            var zipPath = GetZipPath(moduleUniqeName);
            var manifestPath = GetManifestPath(moduleUniqeName);
            var manifest =
                XmlSerializerHelper.Deserialize<ModuleManifest>(File.ReadAllBytes(manifestPath));
            return new ModulePackage()
                       {
                           ModuleManifest = manifest,
                           ModuleZip = File.ReadAllBytes(zipPath)
                       };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        /// TODO: this method probably should not be public!
        public bool AddModule(ModulePackage module)
        {
            try
            {
                var name = module.ModuleManifest.ModuleName;
                var manifestFile = GetManifestPath(name);
                var zipFile = GetZipPath(name);
                var manifestData = XmlSerializerHelper.Serialize(module.ModuleManifest);
                File.WriteAllBytes(manifestFile, manifestData);
                File.WriteAllBytes(zipFile, module.ModuleZip);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GetZipPath(string moduleName)
        {
            return Path.Combine(_modulesDir, "{0}.zip");
        }

        private string GetManifestPath(string moduleName)
        {
            return Path.Combine(_modulesDir,
                                string.Format("{0}{1}", moduleName,
                                              ModuleManifest.ManifestFileNameSuffix));
        }
    }
}
