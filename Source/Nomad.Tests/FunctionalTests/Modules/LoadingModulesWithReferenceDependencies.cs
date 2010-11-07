using System.IO;
using Nomad.Exceptions;
using Nomad.Modules.Discovery;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Modules
{
    [FunctionalTests]
    public class LoadingModulesWithReferenceDependencies : ModuleLoadingWithCompilerTestFixture
    {
        /// <summary>
        ///     ModuleWithDependency -> DependencyModule1 + DependencyModule2
        /// </summary>
        [Test]
        public void loading_one_module_dependent_on_others_callback()
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


        /// <summary>
        ///     ModuleWithDependency -> DependencyModule2 -> DependencyModule1
        /// </summary>
        [Test]
        public void loading_chain_of_depenedent_modules_callback()
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


        /// <summary>
        ///     Chain loading with missing dependency in it.
        /// </summary>
        [Test]
        public void
            loading_module_with_dependency_with_no_dependency_present_results_in_exception_callback()
        {
            const string dir = @"Modules\Dependent3\ModuleA\";
            const string dir2 = @"Modules\Dependent3\ModuleB\";
            const string dir3 = @"Modules\Dependent3\ModuleC\";

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
            // remove dependency
            string[] files = Directory.GetFiles(dir3);
            foreach (string file in files)
            {
                File.Delete(file);
            }

            // define discovery sequence
            var discovery = new CompositeModuleDiscovery(new IModuleDiscovery[]
                                                             {
                                                                 new DirectoryModuleDiscovery(dir2),
                                                                 new DirectoryModuleDiscovery(dir),
                                                                 new DirectoryModuleDiscovery(dir3),
                                                             });

            // perform test and assert
            Assert.Throws<NomadCouldNotLoadModuleException>(
                () => LoadModulesFromDiscovery(discovery));
        }


        [Test]
        public void loading_module_fails_during_initialization_phase_throws_an_exception_callback()
        {
            const string dir1 = @"Modules\Dependent4\ModuleA\";
            const string dir2 = @"Modules\Dependent4\ModuleB\";

            // dependant module generation
            SetUpModuleWithManifest(dir1,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\ErrorInitialize\DependencyModule1.cs");

            // second dependent module generation
            SetUpModuleWithManifest(dir2,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\ErrorInitialize\ModuleWithDependency.cs",
                                    dir1 + "DependencyModule1.dll");

            // define discovery sequence
            var discovery = new CompositeModuleDiscovery(new IModuleDiscovery[]
                                                             {
                                                                 new DirectoryModuleDiscovery(dir2),
                                                                 new DirectoryModuleDiscovery(dir1),
                                                             });

            // perform test and assert
            Assert.Throws<NomadCouldNotLoadModuleException>(
                () => LoadModulesFromDiscovery(discovery));
        }


        [Test]
        public void loading_module_fails_during_loading_assembly_phase_throws_an_exception_callback()
        {
            const string dir1 = @"Modules\Dependent5\ModuleA\";
            const string dir2 = @"Modules\Dependent5\ModuleB\";

            // dependant module generation
            SetUpModuleWithManifest(dir1,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\ErrorLoad\DependencyModule1.cs");

            // second dependent module generation
            SetUpModuleWithManifest(dir2,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\ErrorLoad\ModuleWithDependency.cs",
                                    dir1 + "DependencyModule1.dll");

            // overwriting the dll file, causing BadImageFormatException
            File.Create(dir1 + "DependencyModule1.dll");

            // define discovery sequence
            var discovery = new CompositeModuleDiscovery(new IModuleDiscovery[]
                                                             {
                                                                 new DirectoryModuleDiscovery(dir2),
                                                                 new DirectoryModuleDiscovery(dir1),
                                                             });

            // perform test and assert
            Assert.Throws<NomadCouldNotLoadModuleException>(
                () => LoadModulesFromDiscovery(discovery));
        }
    }
}