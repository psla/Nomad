using System;
using System.IO;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Nomad.Modules;
using Nomad.ServiceLocation;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.ServiceLocation
{
    [FunctionalTests]
    public class ServiceLocatorInModules
    {
        private readonly string _pathToRegistering =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                         @"Modules\ServiceLocatorEnabled\RegistringServiceModule.dll");

        private readonly string _pathToResolving =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                         @"Modules\ServiceLocatorEnabled\ResolvingServiceModule.dll");

        private ModuleLoader _moduleLoader;
        private IServiceLocator _serviceLocator;


        [SetUp]
        public void SetUp()
        {
            ServiceRegistry.Clear();

            var container = new WindsorContainer();
            _moduleLoader = new ModuleLoader(container);
            _serviceLocator = new ServiceLocator(container);

            container.Register(
                Component.For<IServiceLocator>().Instance(_serviceLocator)
                );
        }


        [Test]
        public void
            loading_module_with_service_provider_then_resolving_this_service_by_another_module()
        {
            _moduleLoader.LoadModuleFromFile(_pathToRegistering);

            Assert.AreEqual(1, ServiceRegistry.GetRegisteredServices().Count,
                            "Module has not called the constructor of the service provider class");

            Assert.DoesNotThrow(() => _serviceLocator.Resolve<ITestService>(),
                                "Module has not properly registered service provider");

            Assert.AreEqual(0, ServiceRegistry.GetRegisteredServiceCounter()[typeof (ITestService)],
                            "Method in service has already been used");

            _moduleLoader.LoadModuleFromFile(_pathToResolving);

            Assert.AreEqual(1, ServiceRegistry.GetRegisteredServiceCounter()[typeof (ITestService)],
                            "Method in service was not called exactly one time");
        }


        [Test]
        public void attempt_to_register_by_two_modules_same_service_results_in_exception_event()
        {
            _moduleLoader.LoadModuleFromFile(_pathToRegistering);

            //TODO: Make this test up, when Module Loader Exception Handling Interface will be defined
            //Assert.Throws<ArgumentException>(() => _moduleLoader.LoadModuleFromFile(_pathToRegistering));
        }


        [Test]
        public void
            attempt_to_load_module_with_service_dependency_without_loading_this_dependency_earlier_results_in_exception_event
            ()
        {
            //TODO: Make this test up, when Module Loader Exception Handling Interface will be defined
            //Assert.Throws<ArgumentException>(() => _moduleLoader.LoadModuleFromFile(_pathToResolving));
        }
    }
}