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
        [Test]
        public void empty_domain_returns_empty_list()
        {
            var loadedModulesService = Kernel.ServiceLocator.Resolve<ILoadedModulesService>();

            Assert.AreEqual(0, loadedModulesService.GetLoadedModules().Count);
        }


        [Test]
        public void loaded_modules_appear_on_list()
        {
            const string dir = @"Modules\Services\Module1";
            const string dir2 = @"Modules\Services\Module2";

            SetUpModuleWithManifest(dir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Services\SimplestModulePossible1.cs");

            SetUpModuleWithManifest(dir2,
                                     @"..\Source\Nomad.Tests\FunctionalTests\Data\Services\SimplestModulePossible2.cs");

            // define discovery sequence
            var discovery =
                new CompositeModuleDiscovery(new[]
                                                 {
                                                     new DirectoryModuleDiscovery(dir),
                                                     new DirectoryModuleDiscovery(dir2)
                                                 });
            // perform test and assert
            LoadModulesFromDiscovery(discovery);

            var loadedModulesService = Kernel.ServiceLocator.Resolve<ILoadedModulesService>();

            Assert.AreEqual(2, loadedModulesService.GetLoadedModules().Count);
        }
    }
}