using System.IO;
using Nomad.Modules.Discovery;
using Nomad.Tests.FunctionalTests.Fixtures;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Modules
{
    [FunctionalTests]
    public class LoadingModulesWithServiceDependencies : ModuleLoadingTestFixture
    {
        [SetUp]
        public void SetUp()
        {
            CopyModuleIntoDirectory(
                @"Modules\WithDependencies\ModuleWithConstructorDependency.dll",
                @"Modules\WithDependencies\ModuleWithConstructorDependency\ModuleWithConstructorDependency.dll");
            SignModule(@"ModuleWithConstructorDependency.dll",
                       @"Modules\WithDependencies\ModuleWithConstructorDependency\");

            CopyModuleIntoDirectory(@"Modules\WithDependencies\ModuleWithPropertyDependency.dll",
                                    @"Modules\WithDependencies\ModuleWithPropertyDependency\ModuleWithPropertyDependency.dll");
            SignModule(@"ModuleWithPropertyDependency.dll",
                       @"Modules\WithDependencies\ModuleWithPropertyDependency\");
        }


        [Test]
        public void module_loader_discovers_and_loads_all_simple_modules()
        {
            var compositeDiscovery =
                new CompositeModuleDiscovery(new IModuleDiscovery[]
                                                 {
                                                     new DirectoryModuleDiscovery
                                                         (@"Modules\WithDependencies\ModuleWithConstructorDependency\", SearchOption.TopDirectoryOnly)
                                                     ,
                                                     new DirectoryModuleDiscovery
                                                         (@"Modules\WithDependencies\ModuleWithPropertyDependency\", SearchOption.TopDirectoryOnly)
                                                 });
            LoadModulesFromDirectory(compositeDiscovery);

            AssertModulesLoadedAreEqualTo("ModuleWithConstructorDependency",
                                          "ModuleWithPropertyDependency");
        }


        // TODO: ServiceLocator, EventAggregator and further Nomad Services loading order tests.
    }
}