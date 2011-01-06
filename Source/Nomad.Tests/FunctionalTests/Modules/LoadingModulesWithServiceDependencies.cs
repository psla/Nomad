using System.IO;
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
            Directory.CreateDirectory(@"Modules\WithDependencies\ModuleWithConstructorDependency");
            File.Copy(@"Modules\WithDependencies\ModuleWithConstructorDependency.dll",
                      @"Modules\WithDependencies\ModuleWithConstructorDependency\ModuleWithConstructorDependency.dll",
                      true);
            SignModule(@"ModuleWithConstructorDependency.dll",
                       @"Modules\WithDependencies\ModuleWithConstructorDependency\");

            Directory.CreateDirectory(@"Modules\WithDependencies\ModuleWithPropertyDependency\");
            File.Copy(@"Modules\WithDependencies\ModuleWithPropertyDependency.dll",
                      @"Modules\WithDependencies\ModuleWithPropertyDependency\ModuleWithPropertyDependency.dll",
                      true);
            SignModule(@"ModuleWithPropertyDependency.dll",
                       @"Modules\WithDependencies\ModuleWithPropertyDependency\");
        }


        [Test]
        public void module_loader_discovers_and_loads_all_simple_modules()
        {
            LoadModulesFromDirectory(@"Modules\WithDependencies\ModuleWithConstructorDependency\");
            LoadModulesFromDirectory(@"Modules\WithDependencies\ModuleWithPropertyDependency\");
            AssertModulesLoadedAreEqualTo("ModuleWithConstructorDependency",
                                          "ModuleWithPropertyDependency");
        }


        // TODO: ServiceLocator, EventAggregator and further Nomad Services loading order tests.
    }
}