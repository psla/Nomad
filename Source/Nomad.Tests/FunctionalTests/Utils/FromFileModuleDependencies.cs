using System;
using System.IO;
using System.Linq;
using Nomad.Modules.Manifest;
using Nomad.Utils.ManifestCreator;
using Nomad.Utils.ManifestCreator.DependenciesProvider;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Utils
{
    /// <summary>
    ///     Basic set of tests for the class used as an default for providing the dependencies for <see cref="ManifestBuilder"/>.
    /// </summary>
    [FunctionalTests]
    public class FromFileModuleDependencies
    {
        private const string ConfFileName = @"TEST_DEPENDENCY_FILE_NAME";
        private const string ModuleFileName = @"TEST_MODULE.dll";

        private const string TestPath = @"FunctionalTests\Util\";
        private FromFileModulesDependencyProvider _dependencyProvider;


        [TestFixtureSetUp]
        public void set_up_fixture()
        {
            if (Directory.Exists(TestPath))
                Directory.Delete(TestPath, true);

            Directory.CreateDirectory(TestPath);
        }


        [SetUp]
        public void set_up()
        {
            _dependencyProvider =
                new FromFileModulesDependencyProvider(Path.Combine(TestPath, ConfFileName));

            // copy the psake built module into this place
            File.Copy(@"Modules\Simple\SimplestModulePossible1.dll",
                      Path.Combine(TestPath, ModuleFileName), true);
        }


        [Test]
        public void no_passing_value_in_constructor_uses_the_default_value()
        {
            _dependencyProvider = new FromFileModulesDependencyProvider();
            Assert.AreEqual(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FromFileModulesDependencyProvider.DefaultFileName),
                            _dependencyProvider.DependencyFileName,
                            "Default name is not same as default in constructor");
        }


        [Test]
        public void no_file_in_filesystem_results_in_empty_dependencies()
        {
            Assert.AreEqual(Enumerable.Empty<ModuleDependency>(),
                            _dependencyProvider.GetDependencyModules(TestPath, ModuleFileName),
                            "The collection should be empty");
        }


        [Test]
        public void provider_reutrns_dependencies_as_are_in_conf_file()
        {
            // create the text file with links to dependencies (valid assemblies)
            const string line1 = @"Modules\Simple\SimplestModulePossible1.dll";
            const string line2 = @"Modules\Simple\SimplestModulePossible2.dll";

            File.WriteAllLines(Path.Combine(TestPath, ConfFileName), new[]
                                                                         {
                                                                             line1, line2
                                                                         });

            // try reading this files
            var result = _dependencyProvider.GetDependencyModules(TestPath, ModuleFileName);

            // assert
            Assert.AreEqual(2, result.Count(), "There should be two dependencies on list");
            Assert.AreEqual(new[]{ "SimplestModulePossible1","SimplestModulePossible2"},
                            result.Select( x => x.ModuleName),
                            "The dependencies names are wrong should be name of the classes");
        }


        [Test]
        public void the_file_is_corrupted_resutls_in_exception()
        {
            // corrupted file this is
            var data = new byte[] {0xFF, 0xFF};
            File.WriteAllBytes(Path.Combine(TestPath, ConfFileName), data);

            // act
            Assert.Throws<FileFormatException>(
                () => _dependencyProvider.GetDependencyModules(TestPath, ModuleFileName),
                "The corrupted file should throw FileFormatException");
        }
    }
}