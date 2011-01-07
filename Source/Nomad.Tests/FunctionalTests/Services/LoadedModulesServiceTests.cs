using System;
using System.IO;
using Nomad.Modules.Discovery;
using Nomad.Services;
using Nomad.Tests.FunctionalTests.Fixtures;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Services
{
    [FunctionalTests]
    public class LoadedModulesServiceTests : ModuleLoadingWithCompilerTestFixture
    {
        private const string dir = @"Modules\Services\Module1";
        private const string dir2 = @"Modules\Services\Module2";
        private const string dir3 = @"Modules\Services\VerificationModule";


        [TestFixtureTearDown]
        public override void CleanUpFixture()
        {
            base.CleanUpFixture();
            Kernel.UnloadModules();
            Directory.Delete(@"Modules\Services\", true);
        }


        [Test]
        public void empty_domain_returns_empty_list()
        {
            var loadedModulesService = Kernel.ServiceLocator.Resolve<ILoadedModulesService>();

            Assert.AreEqual(0, loadedModulesService.GetLoadedModules().Count);
        }


        [Test]
        public void loaded_modules_appear_on_list_in_kernel_and_modules_domain()
        {
            SetUpModuleWithManifest(dir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Services\SimplestModulePossible1.cs");

            SetUpModuleWithManifest(dir2,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Services\SimplestModulePossible2.cs");

            SetUpModuleWithManifest(dir3,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Services\LoadedModulesServiceTestingModule.cs");

            // define discovery sequence
            var discovery =
                new CompositeModuleDiscovery(new[]
                                                 {
                                                     new DirectoryModuleDiscovery(dir),
                                                     new DirectoryModuleDiscovery(dir2),
                                                     new DirectoryModuleDiscovery(dir3)
                                                 });
            // perform kernel test and assert
            LoadModulesFromDiscovery(discovery);

            var loadedModulesService = Kernel.ServiceLocator.Resolve<ILoadedModulesService>();

            Assert.AreEqual(3, loadedModulesService.GetLoadedModules().Count);

            //verify from VerificationModule in modules domain
            int loaded;

            using (StreamReader verificationFile =
                File.OpenText(
                    @"Modules\Services\VerificationModule\ILoadedModulesServiceVerificationFile"))
            {
                Int32.TryParse(verificationFile.ReadLine(),
                               out loaded);
            }

            Assert.AreEqual(3, loaded);
        }
    }
}