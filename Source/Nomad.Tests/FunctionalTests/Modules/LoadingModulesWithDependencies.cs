using System;
using System.IO;
using Nomad.Exceptions;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Modules
{
    [FunctionalTests]
    public class LoadingModulesWithDependencies : ModuleLoadingTestFixture
    {
        private ModuleCompiler _moduleCompiler;

        [TestFixtureSetUp]
        public void set_up_fixture()
        {
             _moduleCompiler = new ModuleCompiler();
        }

        [Test]
        public void loading_one_module_dependent_on_one_another()
        {
            const string dir = @"Modules\Dependent1\";
            string modulePath = string.Empty;

            _moduleCompiler.OutputDirectory = dir;
            modulePath = _moduleCompiler.GenerateModuleFromCode(@"..\Source\Nomad.Tests\FunctionalTests\Data\Dependencies\DependencyModule1.cs");
            
            _moduleCompiler.GenerateManifestForModule(modulePath);

            modulePath = _moduleCompiler.GenerateModuleFromCode(@"..\Source\Nomad.Tests\FunctionalTests\Data\Dependencies\ModuleWithDependency.cs",
                                                     "DependencyModule1.dll");

            _moduleCompiler.GenerateManifestForModule(modulePath);

            

            LoadModulesFromDirectory(dir);
            AssertModulesLoadedAreEqualTo("ModuleWithDependency", "DependencyModule1");
        }


        [Test]
        public void loading_one_module_dependents_on_few_modules()
        {
            //LoadModulesFromDirectory(@"Modules\Dependent2\");
            //AssertModulesLoadedAreEqualTo("IndependentModule", "DependentModule1",
            //                              "DependentModule2", "DependentModule3");
            throw new NotImplementedException();
        }


        [Test]
        public void loading_chain_of_depenedent_modules()
        {
            //LoadModulesFromDirectory(@"Modules\Dependent3\");
            //AssertModulesLoadedAreEqualTo("IndependentModule", "DependentModule1",
            //                              "DependentModule2", "DependentModule3");
            throw new NotImplementedException();
        }


        [Test]
        public void loading_two_modules_dependent_on_each_other_throws_an_event()
        {
            throw new NotImplementedException();
        }


        [Test]
        public void loading_module_with_dependency_with_no_dependency_present_results_in_event()
        {
            throw new NotImplementedException();
        }


        [Test]
        public void loading_module_fails_during_initialization_phase_throws_an_event()
        {
            throw new NotImplementedException();
        }


        [Test]
        public void loading_module_fails_during_loading_assembly_phase_throws_an_event()
        {
            throw new NotImplementedException();
        }

    }
}