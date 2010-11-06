using System;
using System.IO;
using System.Security.Policy;
using Nomad.KeysGenerator;
using Nomad.Modules;
using Nomad.Modules.Filters;
using NUnit.Framework;

namespace Nomad.Tests.FunctionalTests.Modules
{
    public class ModuleLoadingWithCompilerTestFixture : ModuleLoadingTestFixture
    {
        private const string KeyFile = @"alaMaKota.xml";
        private ModuleCompiler _moduleCompiler;
        protected AppDomain Domain { get; private set; }


        [TestFixtureSetUp]
        public void set_up_fixture()
        {
            _moduleCompiler = new ModuleCompiler();
            if (File.Exists(KeyFile))
            {
                File.Delete(KeyFile);
            }
            KeysGeneratorProgram.Main(new[] {KeyFile});
        }


        [SetUp]
        public void set_up()
        {
            Manager = new ModuleManager(new ModuleLoader(Container), new CompositeModuleFilter(),
                                       new DependencyChecker());

            Domain = AppDomain.CreateDomain("TEST DOMAIN",
                                            new Evidence(AppDomain.CurrentDomain.Evidence),
                                            AppDomain.CurrentDomain.BaseDirectory, ".",
                                            true);
        }


        protected void SetUpModuleWithManifest(string outputDirectory, string srcPath,
                                               params string[] references)
        {
            _moduleCompiler.OutputDirectory = outputDirectory;

            string modulePath = string.Empty;
            modulePath = _moduleCompiler.GenerateModuleFromCode(srcPath, references);
            _moduleCompiler.GenerateManifestForModule(modulePath, KeyFile);
        }
    }
}