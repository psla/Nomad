using System;
using System.IO;
using Moq;
using Nomad.Modules;
using Nomad.Modules.Manifest;
using NUnit.Framework;
using TestsShared;
using System.Linq;

namespace Nomad.Tests.IntegrationTests.Modules
{
    [IntegrationTests]
    public class SimpleDirectoryModuleDiscovery
    {
        [Test]
        public void discovers_all_assemblies_from_given_directory_and_ignores_other_files()
        {
            var manifestFactoryMock = new Mock<IModuleManifestFactory>(MockBehavior.Loose);
            var manifest = new ModuleManifest();
            manifestFactoryMock.Setup(x => x.GetManifest(It.IsAny<ModuleInfo>())).Returns(manifest);


            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    @"IntegrationTests\DirectoryModuleDiscovery\");

            var expectedModules = new[]
                                      {
                                          new ModuleInfo(Path.Combine(path, "a.dll"),manifestFactoryMock.Object),
                                          new ModuleInfo(Path.Combine(path, "b.dll"),manifestFactoryMock.Object),
                                      };

            var discovery = new Nomad.Modules.Discovery.SimpleDirectoryModuleDiscovery(path);

            Assert.That(discovery.GetModules().ToArray(), Is.EquivalentTo(expectedModules),
                        "Discovered modules differ from expected");
        }
    }
}