using System;
using System.Diagnostics;
using System.IO;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Nomad.Modules;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Modules
{
    [FunctionalTests]
    public class LoadingModulesWithServiceDependencies : ModuleLoadingTestFixture
    {
        [Test]
        public void module_loader_discovers_and_loads_all_simple_modules()
        {
            LoadModulesFromDirectory(@"Modules\WithDependencies\");
            AssertModulesLoadedAreEqualTo("ModuleWithConstructorDependency", "ModuleWithPropertyDependency");
        }
    }
}