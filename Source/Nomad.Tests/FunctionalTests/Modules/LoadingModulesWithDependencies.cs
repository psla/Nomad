using System;
using System.Security.Policy;
using Nomad.Modules.Discovery;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Modules
{
    [FunctionalTests]
    public class LoadingModulesWithDependencies : ModuleLoadingTestFixture
    {
        private ModuleCompiler _moduleCompiler;
        private AppDomain _domain;
        private const string KeyFile = @"alaMaKota.xml";

        [TestFixtureSetUp]
        public void set_up_fixture()
        {
            _moduleCompiler = new ModuleCompiler();
            KeysGenerator.KeysGeneratorProgram.Main(new[] { KeyFile });
        }

        [SetUp]
        public void set_up()
        {
            _domain = AppDomain.CreateDomain("TEST DOMAIN",
                                            new Evidence(AppDomain.CurrentDomain.Evidence),
                                            AppDomain.CurrentDomain.BaseDirectory, ".",
                                            true);  
        }

        private void SetUpModuleWithManifest(string outputDirectory, string srcPath,
                                             params string[] references)
        {
            _moduleCompiler.OutputDirectory = outputDirectory;

            string modulePath = string.Empty;
            modulePath = _moduleCompiler.GenerateModuleFromCode(srcPath, references);
            _moduleCompiler.GenerateManifestForModule(modulePath,KeyFile);
        }


        [Test]
        public void loading_one_module_dependent_on_others()
        {
            _domain.DoCallBack(loading_one_module_dependent_on_others_callback);
        }


        /// <summary>
        ///     ModuleWithDependency -> DependencyModule1 + DependencyModule2
        /// </summary>
        private void loading_one_module_dependent_on_others_callback()
        {
            const string dir = @"Modules\Dependent1\ModuleA\";
            const string dir2 = @"Modules\Dependent1\ModuleB\";
            const string dir3 = @"Modules\Dependent1\ModuleC\";

            // dependant module generation
            SetUpModuleWithManifest(dir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Dependencies\DependencyModule1.cs");

            // second dependent module generation
            SetUpModuleWithManifest(dir3,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Dependencies\DependencyModule2.cs");

            // depending module generation
            SetUpModuleWithManifest(dir2,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Dependencies\ModuleWithDependency.cs",
                                    dir + "DependencyModule1.dll", dir3 + "DependencyModule2.dll");

            // define discovery sequence
            var discovery = new CompositeModuleDiscovery(new IModuleDiscovery[]
                                                             {
                                                                 new DirectoryModuleDiscovery(dir2),
                                                                 new DirectoryModuleDiscovery(dir),
                                                                 new DirectoryModuleDiscovery(dir3),
                                                             });

            // perform test and assert
            LoadModulesFromDiscovery(discovery);
            AssertModulesLoadedAreEqualTo("DependencyModule1", "DependencyModule2",
                                          "ModuleWithDependency");
        }

        [Test]
        public void loading_chain_of_depenedent_modules()
        {
            _domain.DoCallBack(loading_chain_of_depenedent_modules_callback);
        }

        /// <summary>
        ///     ModuleWithDependency -> DependencyModule2 -> DependencyModule1
        /// </summary>
        private void loading_chain_of_depenedent_modules_callback()
        {
            const string dir = @"Modules\Dependent2\ModuleA\";
            const string dir2 = @"Modules\Dependent2\ModuleB\";
            const string dir3 = @"Modules\Dependent2\ModuleC\";

            // dependant module generation
            SetUpModuleWithManifest(dir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\ChainDependencies\DependencyModule1.cs");

            // second dependent module generation
            SetUpModuleWithManifest(dir3,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\ChainDependencies\DependencyModule2.cs",
                                    dir + "DependencyModule1.dll");

            // third dependent module generation
            SetUpModuleWithManifest(dir2,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\ChainDependencies\ModuleWithDependency.cs",
                                    dir3 + "DependencyModule2.dll");

            // define discovery sequence
            var discovery = new CompositeModuleDiscovery(new IModuleDiscovery[]
                                                             {
                                                                 new DirectoryModuleDiscovery(dir2),
                                                                 new DirectoryModuleDiscovery(dir),
                                                                 new DirectoryModuleDiscovery(dir3),
                                                             });

            // perform test and assert
            LoadModulesFromDiscovery(discovery);
            AssertModulesLoadedAreEqualTo("DependencyModule1", "DependencyModule2",
                                          "ModuleWithDependency");
        }


        public void loading_two_modules_dependent_on_each_other_throws_an_exception()
        {
            //TODO: think about this test
        }


        [Test]
        public void loading_module_with_dependency_with_no_dependency_present_results_in_exception()
        {
            throw new NotImplementedException();
        }


        [Test]
        public void loading_module_fails_during_initialization_phase_throws_an_exception()
        {
            throw new NotImplementedException();
        }


        [Test]
        public void loading_module_fails_during_loading_assembly_phase_throws_an_exception()
        {
            throw new NotImplementedException();
        }
    }
}