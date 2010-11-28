using System;
using System.Collections.Generic;
using Moq;
using Nomad.Modules.Discovery;
using Nomad.Updater;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Updater
{
    [UnitTests]
    public class PerformingUpdates : UpdaterTestFixture
    {
        #region Packager invocations

        [Test]
        public void performing_updates_uses_packager_on_one_package_exactly_once()
        {
            var modulePackages = new List<ModulePackage>
                                     {
                                         new ModulePackage()
                                     };

            Updater.PerformUpdates(modulePackages);

            ModulePackager.Verify(x => x.PerformUpdates(PluginsDir, It.IsAny<ModulePackage>()),
                                  Times.Exactly(1), "One package should be invoked then only one.");
        }


        [TestCase(0)]
        [TestCase(5)]
        public void performing_updates_uses_packager_on_n_packages_exactly_n_times(int times)
        {
            int n = times;
            var modulePackages = new List<ModulePackage>();
            for (int i = 0; i < n; i++)
            {
                modulePackages.Add(new ModulePackage());
            }

            Updater.PerformUpdates(modulePackages);

            ModulePackager.Verify(x => x.PerformUpdates(PluginsDir, It.IsAny<ModulePackage>()),
                                  Times.Exactly(n),
                                  string.Format("One package should be invoked {0} times.", n));
        }

        #endregion

        #region Information about outcome

        [Test]
        public void performing_updates_has_information_about_failure()
        {
            throw new NotImplementedException();
        }


        [Test]
        public void performing_updates_has_information_about_success()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Basic operations

        [Test]
        public void performing_updates_unload_modules()
        {
            Updater.PerformUpdates(new List<ModulePackage>());
            ModulesOperations.Verify(x => x.UnloadModules(), Times.Exactly(1));
        }


        [Test]
        public void performing_updates_load_modules_back()
        {
            Updater.PerformUpdates(new List<ModulePackage>());
            ModulesOperations.Verify(x => x.LoadModules(It.IsAny<IModuleDiscovery>()),
                                     Times.Exactly(1));
        }

        #endregion
    }
}