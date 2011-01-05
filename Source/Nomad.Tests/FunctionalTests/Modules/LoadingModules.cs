using Nomad.Tests.FunctionalTests.Fixtures;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Modules
{
    [FunctionalTests]
    public class LoadingModules : ModuleLoadingTestFixture
    {
        [SetUp]
        public void SetUp()
        {
            SignModule("SimplestModulePossible1.dll", @"Modules\Simple\");
            SignModule("SimplestModulePossible2.dll", @"Modules\Simple\");
        }


        [Test]
        public void module_loader_discovers_and_loads_all_simple_modules()
        {
            LoadModulesFromDirectory(@"Modules\Simple\");
            AssertModulesLoadedAreEqualTo("SimplestModulePossible1", "SimplestModulePossible2");
        }


        [Test]
        public void module_loader_invokes_onUnLoad_method_in_modules()
        {
            LoadModulesFromDirectory(@"Modules\Simple\");
            InvokeUnloadMethod();
            AssertInvokeUnloadMethodsWasInvoked("SimplestModulePossible1", "SimplestModulePossible2");
        }
    }
}