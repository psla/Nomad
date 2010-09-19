using System;
using System.IO;
using Nomad.Modules;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Modules
{
    [FunctionalTests]
    public class LoadingModulesWithDependencies
    {
        private ModuleLoader _moduleLoader;
        private InjectableModulesRegistry _modulesRegistry;

        [SetUp]
        public void Setup()
        {
            LoadedModulesRegistry.Clear();
            _moduleLoader = new ModuleLoader();
            _modulesRegistry = new InjectableModulesRegistry();
        }
        
        [Test]
        public void loads_module_with_constructor_dependencies()
        {
            var libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           @"Modules\WithDependency\ModuleWithConstructorDependency.dll");
            _moduleLoader.LoadModuleFromFile(libraryPath);
            var registeredModules = _modulesRegistry.GetRegisteredModules();
            Assert.AreEqual(1, registeredModules.Count);
        }

        [Test]
        public void loads_module_with_property_dependencies()
        {
            var libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           @"Modules\WithDependency\ModuleWithPropertyDependencies.dll");
            _moduleLoader.LoadModuleFromFile(libraryPath);
            var registeredModules = _modulesRegistry.GetRegisteredModules();
            Assert.AreEqual(1, registeredModules.Count);
        }
    }
}