using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.Updater;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Updater
{
    [IntegrationTests]
    public class PerformingUpdates : UpdaterTestFixture
    {

        #region Packager invocations
        
       [SetUp]
       public void set_up()
       {
           // sick creating of dependency checker
           IEnumerable<ModuleInfo> outter = new List<ModuleInfo>();
           DependencyChecker.Setup(
               x =>
               x.CheckModules(It.IsAny<IEnumerable<ModuleInfo>>(),
                              It.IsAny<IEnumerable<ModuleInfo>>(),
                              out outter))
               .Returns(true);

           ModuleFinder
               .Setup(x => x.FindDirectoryForPackage(It.IsAny<string>(), It.IsAny<ModulePackage>()))
               .Returns<string,ModulePackage>( (dict,package) => dict );
       }


        [TestCase(0)]
        [TestCase(5)]
        public void performing_updates_uses_packager_on_n_packages_exactly_n_times(int times)
        {
            int n = times;
            IList<ModuleManifest> modulePackages = new List<ModuleManifest>();
            for (int i = 0; i < n; i++)
            {
                modulePackages.Add(new ModuleManifest(){ModuleName = "Test" + i});
            }

            // provide the proper packages per modules
            ModulesRepository.Setup(x => x.GetModule(It.IsAny<string>()))
                .Returns<string>(name => new ModulePackage()
                                             {
                                                 ModuleManifest = modulePackages
                                                                        .Where( x => x.ModuleName.Equals(name))
                                                                        .Select(x => x).Single()
                                             });

            NomadUpdater.PrepareUpdate(modulePackages);

            NomadUpdater.PerformUpdates(new CompositeModuleDiscovery());

            NomadUpdater.UpdateFinished.WaitOne();
            ModulePackager.Verify(x => x.PerformUpdates(PluginsDir, It.IsAny<ModulePackage>()),
                                  Times.Exactly(n),
                                  string.Format("One package should be invoked {0} times.", n));
        }

        #endregion

        #region Silent errors. 

        [Test]
        public void performing_updates_has_information_about_failure_modules_operations_throws()
        {
            ModulesOperations.Setup(x => x.LoadModules(It.IsAny<IModuleDiscovery>()))
                .Throws(new Exception("Can not discovery moduels"));

            var moduleManifests = new List<ModuleManifest>
                             {
                                 new ModuleManifest(),
                             };
            // preapre stub module manifest
            NomadUpdater.PrepareUpdate(moduleManifests);

            ModulesRepository.Setup(x => x.GetModule(It.IsAny<string>()))
                .Returns(new ModulePackage() {ModuleManifest = moduleManifests[0]});

            NomadUpdater.PerformUpdates(new CompositeModuleDiscovery());

            // wait till end to get the information about failure
            NomadUpdater.UpdateFinished.WaitOne();
            Assert.AreEqual(UpdaterStatus.Invalid, NomadUpdater.Status);
        }


        [Test]
        public void performing_updates_has_information_about_failure_packager_throws()
        {
            ModulePackager.Setup(
                x => x.PerformUpdates(It.IsAny<String>(), It.IsAny<ModulePackage>()))
                .Throws(new Exception("Can not pacakge this"));

            var manifest = new ModuleManifest() {ModuleName = "AlaMaKota"};
            var moduleManifests = new List<ModuleManifest>
                             {
                                 manifest,
                             };

            ModulesRepository.Setup(x => x.GetModule(It.IsAny<string>()))
                .Returns(new ModulePackage() {ModuleManifest = manifest});

            // preapre stub module manifest
            NomadUpdater.PrepareUpdate(moduleManifests);

            ModulesRepository.Setup(x => x.GetModule(It.IsAny<string>()))
                .Returns(new ModulePackage() { ModuleManifest = moduleManifests[0] });

            NomadUpdater.PerformUpdates(new CompositeModuleDiscovery());

            // wait till end to get the information about failure
            NomadUpdater.UpdateFinished.WaitOne();
            Assert.AreEqual(UpdaterStatus.Invalid, NomadUpdater.Status);
        }


        [Test]
        public void performing_updates_has_information_about_success()
        {
            NomadUpdater.PerformUpdates(new CompositeModuleDiscovery());

            NomadUpdater.UpdateFinished.WaitOne();
            Assert.AreEqual(UpdaterStatus.Idle, NomadUpdater.Status);
        }

        #endregion

        #region Basic operations scenarios

        [Test]
        public void performing_updates_unload_modules()
        {
            NomadUpdater.PerformUpdates(new CompositeModuleDiscovery());

            NomadUpdater.UpdateFinished.WaitOne();
            ModulesOperations.Verify(x => x.UnloadModules(), Times.Exactly(1));
        }


        [Test]
        public void performing_updates_load_modules_back()
        {
            NomadUpdater.PerformUpdates(new CompositeModuleDiscovery());

            NomadUpdater.UpdateFinished.WaitOne();
            ModulesOperations.Verify(x => x.LoadModules(It.IsAny<IModuleDiscovery>()),
                                     Times.Exactly(1));
        }

        #endregion
    }
}