using System;
using System.IO;
using Nomad.Modules;
using NUnit.Framework;
using TestsShared;
using System.Linq;

namespace Nomad.Tests.IntegrationTests.Modules
{
    [IntegrationTests]
    public class DirectoryModuleDiscovery
    {
        [Test]
        public void discovers_all_modules_from_given_directory_and_ignores_other_files()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    @"IntegrationTests\DirectoryModuleDiscovery\");
            var expectedModules = new[]
                                      {
                                          new ModuleInfo(Path.Combine(path, "a.dll")),
                                          new ModuleInfo(Path.Combine(path, "b.dll"))
                                      };

            var discovery = new Nomad.Modules.DirectoryModuleDiscovery(path);

            Assert.That(discovery.GetModules().ToArray(), Is.EquivalentTo(expectedModules),
                        "Discovered modules differ from expected");
        }
    }
}