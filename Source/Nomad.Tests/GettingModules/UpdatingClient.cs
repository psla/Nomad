using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using Nomad.Core;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.ModulesRepository.Data;
using Nomad.Updater;
using NUnit.Framework;
using TestsShared;
using Version = Nomad.Utils.Version;

namespace Nomad.Tests.GettingModules
{
    [UnitTests]
    public class UpdatingClient
    {
        private Mock<IModuleDiscovery> _moduleDiscovery;
        private Mock<IModuleManifestFactory> _moduleManifestFactory;
        private Mock<IModulesOperations> _modulesOperations;
        private Mock<IModulesRepository> _modulesRepository;
        private string _pluginsDir;
        private Updater.Updater _updateClient;


        [SetUp]
        public void Setup()
        {
            _modulesRepository = new Mock<IModulesRepository>();
            _modulesOperations = new Mock<IModulesOperations>();
            _moduleDiscovery = new Mock<IModuleDiscovery>();
            _moduleManifestFactory = new Mock<IModuleManifestFactory>();
            _pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateTests");

            if (Directory.Exists(_pluginsDir))
                Directory.Delete(_pluginsDir, true);
            Directory.CreateDirectory(_pluginsDir);

            _updateClient = new Updater.Updater(_pluginsDir, _modulesRepository.Object,
                                                _modulesOperations.Object,
                                                _moduleDiscovery.Object,
                                                _moduleManifestFactory.Object);
        }


        [Test]
        public void discovers_module_updates_when_they_are()
        {
            AvailableUpdatesEventArgs payload = null;
            string assembly = "test_assembly.dll";
            var moduleInfo = new ModuleInfo(assembly);
            var moduleManifest = new ModuleManifest
                                     {
                                         ModuleName = assembly,
                                         ModuleVersion = new Version("0.0.0.0")
                                     };
            var updateManifest = new ModuleManifest
                                     {
                                         ModuleName = assembly,
                                         ModuleVersion = new Version("0.0.0.1")
                                     };
            _moduleDiscovery.Setup(x => x.GetModules()).Returns(new List<ModuleInfo>
                                                                    {
                                                                        moduleInfo
                                                                    });
            _moduleManifestFactory.Setup(x => x.GetManifest(It.Is<ModuleInfo>(y => y == moduleInfo)))
                .Returns(moduleManifest);

            _modulesRepository.
                Setup(x => x.GetAvailableModules()).
                Returns(new AvailableModules
                            {
                                Manifests = new List<ModuleManifest> {updateManifest}
                            });

            _updateClient.AvailableUpdates += (x, y) => payload = y;
            _updateClient.CheckUpdates();

            Assert.AreEqual(1, payload.AvailableUpdates.Count,
                            "There should be one module to update");
            Assert.AreSame(updateManifest, payload.AvailableUpdates.First());
        }


        [Test]
        public void does_not_inform_about_modules_which_have_same_version()
        {
            AvailableUpdatesEventArgs payload = null;
            string assembly = "test_assembly.dll";
            var moduleInfo = new ModuleInfo(assembly);
            var moduleManifest = new ModuleManifest
                                     {
                                         ModuleName = assembly,
                                         ModuleVersion = new Version("0.0.0.1")
                                     };
            var updateManifest = new ModuleManifest
                                     {
                                         ModuleName = assembly,
                                         ModuleVersion = new Version("0.0.0.1")
                                     };
            _moduleDiscovery.Setup(x => x.GetModules()).Returns(new List<ModuleInfo>
                                                                    {
                                                                        moduleInfo
                                                                    });
            _moduleManifestFactory.Setup(x => x.GetManifest(It.Is<ModuleInfo>(y => y == moduleInfo)))
                .Returns(moduleManifest);

            _modulesRepository.
                Setup(x => x.GetAvailableModules()).
                Returns(new AvailableModules
                            {
                                Manifests = new List<ModuleManifest> {updateManifest}
                            });

            _updateClient.AvailableUpdates += (x, y) => payload = y;
            _updateClient.CheckUpdates();

            Assert.AreEqual(0, payload.AvailableUpdates.Count, "There should be no updates");
        }


        [Test]
        public void adds_dependencies_which_are_not_installed_during_update()
        {
            UpdatesReadyEventArgs payload = null;
            string moduleName = "test_module.dll";
            string dependencyModuleName = "other_module.dll";

            var updateManifest = new ModuleManifest
                                     {
                                         ModuleName = moduleName,
                                         ModuleVersion = new Version("0.0.0.1"),
                                         ModuleDependencies = new List<ModuleDependency>
                                                                  {
                                                                      new ModuleDependency
                                                                          {
                                                                              MinimalVersion =
                                                                                  new Version(
                                                                                  "0.0.0.0"),
                                                                              ModuleName =
                                                                                  dependencyModuleName
                                                                          }
                                                                  }
                                     };
            var dependencyModuleManifest = new ModuleManifest {ModuleName = dependencyModuleName};
            var availableUpdates =
                new AvailableUpdatesEventArgs(new List<ModuleManifest> {updateManifest});

            _modulesRepository.Setup(x => x.GetModule(It.Is<string>(y => y == moduleName))).Returns(
                new ModulePackage {ModuleManifest = updateManifest});

            _modulesRepository.Setup(x => x.GetModule(It.Is<string>(y => y == dependencyModuleName)))
                .Returns(
                    new ModulePackage {ModuleManifest = dependencyModuleManifest});

            _updateClient.UpdatesReady += (x, y) => payload = y;
            _updateClient.PrepareUpdate(availableUpdates);

            Assert.AreEqual(2, payload.ModulePackages.Count,
                            "There should be two modules to update. Module, and module dependency");
            Assert.Contains(updateManifest,
                            payload.ModulePackages.Select(x => x.ModuleManifest).ToList());
            Assert.Contains(dependencyModuleManifest,
                            payload.ModulePackages.Select(x => x.ModuleManifest).ToList());
        }


        [Test]
        public void performing_updates_unzips_data_to_plugin_directory()
        {
            string zipFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"res\test.zip");
            var modulePackages = new List<ModulePackage>
                                     {
                                         new ModulePackage
                                             {
                                                 ModuleManifest = new ModuleManifest(),
                                                 ModuleZip = File.ReadAllBytes(zipFile)
                                             }
                                     };
            _updateClient.PerformUpdates(modulePackages);
            Assert.IsTrue(File.Exists(Path.Combine(_pluginsDir, "readme")));
        }


        [Test]
        public void performing_updates_unload_modules()
        {
            _updateClient.PerformUpdates(new List<ModulePackage>());
            _modulesOperations.Verify(x => x.UnloadModules(), Times.Exactly(1));
        }


        [Test]
        public void performing_updates_load_modules_back()
        {
            _updateClient.PerformUpdates(new List<ModulePackage>());
            _modulesOperations.Verify(x => x.LoadModules(It.IsAny<IModuleDiscovery>()),
                                      Times.Exactly(1));
        }
    }
}