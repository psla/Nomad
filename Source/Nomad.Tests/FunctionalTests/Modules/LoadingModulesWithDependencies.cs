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
    //[FunctionalTests]
    //[Timeout(10000)] 
    public class LoadingModulesWithDependencies
    {
        private ModuleLoader _moduleLoader;
        private InjectableModulesRegistry _modulesRegistry;

        [SetUp]
        public void Setup()
        {
            LoadedModulesRegistry.Clear();
            var container = new WindsorContainer();
            _moduleLoader = new ModuleLoader(container);
            _modulesRegistry = new InjectableModulesRegistry();

            container.Register(
                Component.For<IInjectableModulesRegistry>().Instance(_modulesRegistry)
                );
        }
        
        [Test]
        public void loads_module_with_constructor_dependencies()
        { 
            var libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           @"Modules\WithDependencies\ModuleWithConstructorDependency.dll");
            _moduleLoader.LoadModuleFromFile(libraryPath);
            var registeredModules = _modulesRegistry.GetRegisteredModules();
            Assert.AreEqual(1, registeredModules.Count);
        }

        [Test]
        public void loads_module_with_property_dependencies()
        {
            var libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           @"Modules\WithDependencies\ModuleWithPropertyDependency.dll");
            _moduleLoader.LoadModuleFromFile(libraryPath);
            var registeredModules = _modulesRegistry.GetRegisteredModules();
            Assert.AreEqual(1, registeredModules.Count);
        }
    }
}