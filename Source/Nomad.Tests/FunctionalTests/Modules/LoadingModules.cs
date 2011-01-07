using System.IO;
using Nomad.Modules.Discovery;
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
            CopyModuleIntoDirectory(@"Modules\Simple\SimplestModulePossible1.dll",
                                    @"Modules\Simple\SimplestModulePossible1\SimplestModulePossible1.dll");
            SignModule("SimplestModulePossible1.dll", @"Modules\Simple\SimplestModulePossible1\");

            CopyModuleIntoDirectory(@"Modules\Simple\SimplestModulePossible2.dll",
                                    @"Modules\Simple\SimplestModulePossible2\SimplestModulePossible2.dll");
            SignModule("SimplestModulePossible2.dll", @"Modules\Simple\SimplestModulePossible2\");
        }


        [Test]
        public void module_loader_discovers_and_loads_all_simple_modules()
        {
            LoadSimpleModules();
            AssertModulesLoadedAreEqualTo("SimplestModulePossible1", "SimplestModulePossible2");
        }


        [Test]
        public void module_loader_invokes_onUnLoad_method_in_modules()
        {
            LoadSimpleModules();
            InvokeUnloadMethod();
            AssertInvokeUnloadMethodsWasInvoked("SimplestModulePossible1", "SimplestModulePossible2");
        }


        private void LoadSimpleModules()
        {
            var compositeDiscovery =
                new CompositeModuleDiscovery(new IModuleDiscovery[]
                                                 {
                                                     new DirectoryModuleDiscovery
                                                         (@"Modules\Simple\SimplestModulePossible1\", SearchOption.TopDirectoryOnly)
                                                     ,
                                                     new DirectoryModuleDiscovery
                                                         (@"Modules\Simple\SimplestModulePossible2\", SearchOption.TopDirectoryOnly)
                                                 });
            LoadModulesFromDirectory(compositeDiscovery);
        }
    }
}