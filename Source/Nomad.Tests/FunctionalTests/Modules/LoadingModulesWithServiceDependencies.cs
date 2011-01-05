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
            SignModule(@"ModuleWithConstructorDependency.dll", @"Modules\WithDependencies\");
            SignModule(@"ModuleWithPropertyDependency.dll", @"Modules\WithDependencies\");
        }


        [Test]
        public void module_loader_discovers_and_loads_all_simple_modules()
        {
            LoadModulesFromDirectory(@"Modules\WithDependencies\");
            AssertModulesLoadedAreEqualTo("ModuleWithConstructorDependency",
                                          "ModuleWithPropertyDependency");
        }


        // TODO: ServiceLocator, EventAggregator and further Nomad Services loading order tests.
    }
}