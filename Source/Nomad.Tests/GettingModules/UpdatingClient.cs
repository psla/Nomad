using System.Collections.Generic;
using Moq;
using Nomad.Core;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.ModulesRepository.Data;
using Nomad.Updater;
using Nomad.Utils;
using NUnit.Framework;
using TestsShared;
using System.Linq;

namespace Nomad.Tests.GettingModules
{
    [UnitTests]
    public class UpdatingClient
    {
        private Mock<IModuleDiscovery> _moduleDiscovery;
        private Mock<IModuleManifestFactory> _moduleManifestFactory;
        private Mock<IModulesOperations> _modulesOperations;
        private Mock<IModulesRepository> _modulesRepository;
        private Updater.Updater _updateClient;


        [SetUp]
        public void Setup()
        {
            _modulesRepository = new Mock<IModulesRepository>();
            _modulesOperations = new Mock<IModulesOperations>();
            _moduleDiscovery = new Mock<IModuleDiscovery>();
            _moduleManifestFactory = new Mock<IModuleManifestFactory>();
            _updateClient = new Updater.Updater(_modulesRepository.Object, _modulesOperations.Object,
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
                                Manifests = new List<ModuleManifest> { updateManifest }
                            });

            _updateClient.AvailableUpdates += (x, y) => payload = y;
            _updateClient.CheckUpdates();

            Assert.AreEqual(1, payload.AvailableUpdates.Count, "There should be one module to update");
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
                                Manifests = new List<ModuleManifest> { updateManifest }
                            });

            _updateClient.AvailableUpdates += (x, y) => payload = y;
            _updateClient.CheckUpdates();

            Assert.AreEqual(0, payload.AvailableUpdates.Count, "There should be no updates");
        }

    }
}