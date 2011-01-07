using System;
using System.IO;
using System.Linq;
using Nomad.KeysGenerator;
using Nomad.Modules;
using Nomad.Tests.FunctionalTests.Fixtures;
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

            var discovery = new Nomad.Modules.Discovery.DirectoryModuleDiscovery(testPath,SearchOption.TopDirectoryOnly);

            Assert.That(discovery.GetModules().ToArray(), Is.EquivalentTo(expectedModules),
                        "Discovered modules differ from expected");
        }

        [Test]
        public void discovers_all_proper_modules_recursively_fails()
        {
            // make another folder 
            string testPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           @"IntegrationTests\DirectoryModuleDiscovery2\");
            string moduleAPath = Path.Combine(testPath, "ModuleA");
            string moduleBPath = Path.Combine(testPath, "ModuleB");

            PrepareModulesTestDirectories(testPath, moduleAPath, moduleBPath);
            var expectedModules = new[]
                                      {
                                          new ModuleInfo(Path.Combine(moduleAPath, "a.dll")),
                                          new ModuleInfo(Path.Combine(moduleBPath, "b.dll")),
                                      };

            var discovery = new Nomad.Modules.Discovery.DirectoryModuleDiscovery(testPath, SearchOption.TopDirectoryOnly);

            Assert.IsEmpty(discovery.GetModules().ToArray());
                        
        }

        [Test]
        public void discovers_all_proper_modules_recursively_works()
        {
            // make another folder 
            string testPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           @"IntegrationTests\DirectoryModuleDiscovery2\");
            string moduleAPath = Path.Combine(testPath, "ModuleA");
            string moduleBPath = Path.Combine(testPath, "ModuleB");

            PrepareModulesTestDirectories(testPath, moduleAPath, moduleBPath);

            var expectedModules = new[]
                                      {
                                          new ModuleInfo(Path.Combine(moduleAPath, "a.dll")),
                                          new ModuleInfo(Path.Combine(moduleBPath, "b.dll")),
                                      };

            var discovery = new Nomad.Modules.Discovery.DirectoryModuleDiscovery(testPath, SearchOption.AllDirectories);

            Assert.That(discovery.GetModules().ToArray(), Is.EquivalentTo(expectedModules),
                        "Discovered modules differ from expected");
        }


        private void PrepareModulesTestDirectories(string testPath, string moduleAPath, string moduleBPath)
        {
            if (Directory.Exists(testPath))
                Directory.Delete(testPath, true);
            Directory.CreateDirectory(testPath);

            // compile modules into a.dll / b.dll
            var compiler = new ModuleCompiler();

            
            compiler.OutputDirectory = moduleAPath;
            compiler.OutputName = Path.Combine(moduleAPath, "a.dll");
            compiler.GenerateModuleFromCode(
                @"..\Source\Nomad.Tests\FunctionalTests\Data\SimplestModulePossible1.cs");

            compiler.OutputDirectory = moduleBPath;
            compiler.OutputName = Path.Combine(moduleBPath, "b.dll");
            compiler.GenerateModuleFromCode(
                @"..\Source\Nomad.Tests\FunctionalTests\Data\SimplestModulePossible1.cs");

            // generate manifests
            var builder = new ManifestBuilder(@"TEST_ISSUER", KeyFile, @"a.dll", moduleAPath);
            builder.CreateAndPublish();
            builder = new ManifestBuilder(@"TEST_ISSUER", KeyFile, @"b.dll", moduleBPath);
            builder.CreateAndPublish();

            // add spoiling module (assembly without manifest)
            File.Copy(Path.Combine(moduleAPath, @"a.dll"), Path.Combine(testPath, "c.dll"));
        }
    }
}