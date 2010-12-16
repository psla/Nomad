using System;
using System.IO;
using System.Linq;
using Nomad.KeysGenerator;
using Nomad.Modules;
using Nomad.Tests.FunctionalTests.Modules;
using Nomad.Utils;
using Nomad.Utils.ManifestCreator;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Modules
{
    [IntegrationTests]
    public class DirectoryModuleDiscovery
    {
        private const string KeyFile = @"KEY_FILE.xml";


        [TestFixtureSetUp]
        public void fixture_set_up()
        {
            if (File.Exists(KeyFile))
            {
                File.Delete(KeyFile);
            }
            KeysGeneratorProgram.Main(new[] {KeyFile});
        }


        [Test]
        public void discovers_all_proper_modules_with_manifests_ignores_others_assemblies()
        {
            // make another folder 
            string testPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           @"IntegrationTests\DirectoryModuleDiscovery2\");

            if (Directory.Exists(testPath))
                Directory.Delete(testPath, true);
            Directory.CreateDirectory(testPath);

            // compile modules into a.dll / b.dll
            var compiler = new ModuleCompiler();
            compiler.OutputDirectory = testPath;

            compiler.OutputName = Path.Combine(testPath, "a.dll");
            compiler.GenerateModuleFromCode(
                @"..\Source\Nomad.Tests\FunctionalTests\Data\SimplestModulePossible1.cs");

            compiler.OutputName = Path.Combine(testPath, "b.dll");
            compiler.GenerateModuleFromCode(
                @"..\Source\Nomad.Tests\FunctionalTests\Data\SimplestModulePossible1.cs");

            // generate manifests
            var builder = new ManifestBuilder(@"TEST_ISSUER", KeyFile, @"a.dll", testPath);
            builder.CreateAndPublish();
            builder = new ManifestBuilder(@"TEST_ISSUER", KeyFile, @"b.dll", testPath);
            builder.CreateAndPublish();

            // add spoiling module (assembly without manifest)
            File.Copy(Path.Combine(testPath, @"a.dll"), Path.Combine(testPath, "c.dll"));

            var expectedModules = new[]
                                      {
                                          new ModuleInfo(Path.Combine(testPath, "a.dll")),
                                          new ModuleInfo(Path.Combine(testPath, "b.dll")),
                                      };

            var discovery = new Nomad.Modules.Discovery.DirectoryModuleDiscovery(testPath);

            Assert.That(discovery.GetModules().ToArray(), Is.EquivalentTo(expectedModules),
                        "Discovered modules differ from expected");
        }
    }
}