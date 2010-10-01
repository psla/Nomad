using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Modules
{
    [FunctionalTests]
    public class LoadingModules : ModuleLoadingTestFixture
    {
        [Test]
        public void module_loader_discovers_and_loads_all_simple_modules()
        {
            LoadModulesFromDirectory(@"Modules\Simple\");
            AssertModulesLoadedAreEqualTo("SimplestModulePossible1", "SimplestModulePossible2");
        }
    }
}