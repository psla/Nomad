using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Moq;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Filters;
using NUnit.Framework;

namespace Nomad.Tests.FunctionalTests.Modules
{
    public abstract class ModuleLoadingTestFixture : MarshalByRefObject
    {
        protected WindsorContainer Container;
        protected ModuleManager Manager;
        private InjectableModulesRegistry _registry;


        [SetUp]
        public void clear_registries_and_create_module_manager()
        {
            _registry = new InjectableModulesRegistry();
            LoadedModulesRegistry.Clear();

            Container = new WindsorContainer();
            Container.Register(
                Component.For<IInjectableModulesRegistry>().Instance(_registry)
                );

            var dependencyCheckerMock = new Mock<IDependencyChecker>(MockBehavior.Loose);
            dependencyCheckerMock.Setup(x => x.SortModules(It.IsAny<IEnumerable<ModuleInfo>>()))
                .Returns<IEnumerable<ModuleInfo>>(e => e);

            Manager = new ModuleManager(new ModuleLoader(Container), new CompositeModuleFilter(),
                                        dependencyCheckerMock.Object);
        }


        protected void InvokeUnloadMethod()
        {
            Manager.InvokeUnloadCallback();
        }


        protected void AssertInvokeUnloadMethodsWasInvoked(params string[] expectedModuleNames)
        {
            string[] unloadedModuleNames = LoadedModulesRegistry.GetUnRegisteredModules()
                .Select(type => type.Name)
                .ToArray();

            Assert.That(unloadedModuleNames, Is.EqualTo(expectedModuleNames));
        }


        protected void LoadModulesFromDirectory(string moduleDirectory)
        {
            string fullDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                moduleDirectory);
            Manager.LoadModules(new SimpleDirectoryModuleDiscovery(fullDirectory));
        }


        protected void LoadModulesFromDiscovery(IModuleDiscovery discovery)
        {
            Manager.LoadModules(discovery);
        }


        protected void AssertModulesLoadedAreEqualTo(params string[] expectedModuleNames)
        {
            string[] loadedModulesNames = LoadedModulesRegistry.GetRegisteredModules()
                .Concat(_registry.GetRegisteredModules())
                .Select(type => type.Name)
                .ToArray();

            Assert.That(loadedModulesNames, Is.EqualTo(expectedModuleNames));
        }
    }
}