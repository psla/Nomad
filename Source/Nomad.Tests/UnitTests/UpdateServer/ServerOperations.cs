using System.IO;
using Nomad.Modules.Manifest;
using Nomad.Updater;
using Nomad.Utils;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.UpdateServer
{
    [UnitTests]
    public class ServerOperations
    {
        private ModulesRepository.ModulesRepository _moduleRepository;

        [SetUp]
        public void SetUp()
        {
            _moduleRepository = new ModulesRepository.ModulesRepository();
            //TODO: What the f. does this cleanup?
            //Directory.Delete(_moduleRepository.ModulesDir, true); //cleanup
            //_moduleRepository = new ModulesRepository.ModulesRepository();
        }

        [Test]
        public void added_module_is_shown_in_available_modules()
        {
            var package = new ModulePackage()
                              {
                                  ModuleManifest = new ModuleManifest() {ModuleName = "abrakadabra"},
                                  ModuleZip = new byte[] {1}
                              };
            _moduleRepository.AddModule(package);
            var availableModules = _moduleRepository.GetAvailableModules();

            Assert.AreEqual(1, availableModules.Manifests.Count, "There should be one available item");
        }

        [Test]
        public void added_module_may_be_retrieved()
        {
            string moduleName = "abrakadabra";
            var package = new ModulePackage()
            {
                ModuleManifest = new ModuleManifest() { ModuleName = moduleName },
                ModuleZip = new byte[] { 1 }
            };
            _moduleRepository.AddModule(package);
            var retrieved = _moduleRepository.GetModule(moduleName);

            Assert.AreEqual(package.ModuleZip, retrieved.ModuleZip, "zip data should be equal in both cases");
            Assert.AreEqual(moduleName, retrieved.ModuleManifest.ModuleName);
        }

        [Test]
        public void adding_same_module_twice_and_retrieving_results_in_the_latter()
        {
            //arrange
            string moduleName = "abrakadabra";
            var version1 = new Version("0.0.0.0");
            var version2 = new Version("0.0.0.1");
            var manifest1 = new ModuleManifest() {ModuleName = moduleName, ModuleVersion = version1};
            var manifest2 = new ModuleManifest() {ModuleName = moduleName, ModuleVersion = version2};
            var data1 = new byte[] {1};
            var data2 = new byte[] {2};
            var package1 = new ModulePackage() {ModuleManifest = manifest1, ModuleZip = data1};
            var package2 = new ModulePackage() {ModuleManifest = manifest2, ModuleZip = data2};
            
            //act
            _moduleRepository.AddModule(package1);
            _moduleRepository.AddModule(package2);
            var retrieved = _moduleRepository.GetModule(moduleName);

            //verify
            Assert.AreEqual(data2, retrieved.ModuleZip);
            Assert.AreEqual(version2, retrieved.ModuleManifest.ModuleVersion);
        }

    }
}