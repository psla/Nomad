using System;
using System.IO;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Nomad.Modules;
using NUnit.Framework;

namespace Nomad.Tests.FunctionalTests.Modules
{
    public abstract class ModuleLoadingTestFixture
    {
        private InjectableModulesRegistry _registry;
        private ModuleManager _manager;
        public WindsorContainer Container;


        [SetUp]
        public void clear_registries_and_create_module_manager()
        {
            _registry = new InjectableModulesRegistry();
            LoadedModulesRegistry.Clear();

            Container = new WindsorContainer();
            Container.Register(
                Component.For<IInjectableModulesRegistry>().Instance(_registry)
                );

            _manager = new ModuleManager(new ModuleLoader(Container), new CompositeModuleFilter());
        }


        protected void LoadModulesFromDirectory(string moduleDirectory)
        {
            var fullDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, moduleDirectory);
            _manager.LoadModules(new DirectoryModuleDiscovery(fullDirectory));
        }


        protected void AssertModulesLoadedAreEqualTo(params string[] expectedModuleNames)
        {
            var loadedModulesNames = LoadedModulesRegistry.GetRegisteredModules()
                .Concat(_registry.GetRegisteredModules())
                .Select(type => type.Name)
                .ToArray();

            Assert.That(loadedModulesNames, Is.EqualTo(expectedModuleNames));
        }
    }
}