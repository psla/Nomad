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
        private IServiceLocator _serviceLocator;
        private ModuleLoader _moduleLoader;

        private string _pathToRegistering = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                 @"Modules\ServiceLocatorEnabled\RegistringServiceModule.dll");

        private string _pathToResolving = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                               @"Modules\ServiceLocatorEnabled\ResolvingServiceModule.dll");

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
        public void one_module_registering_one_module_resolving()
        {
            _moduleLoader.LoadModuleFromFile(_pathToRegistering);

            Assert.AreEqual(1, ServiceRegistry.GetRegisteredServices().Count);

            var serviceProvided =  _serviceLocator.Resolve<ITestService>();
            Assert.NotNull(serviceProvided);
            
            Assert.AreEqual(0,ServiceRegistry.GetRegisteredServiceCounter()[typeof(ITestService)]);
            _moduleLoader.LoadModuleFromFile(_pathToResolving);
            Assert.AreEqual(1, ServiceRegistry.GetRegisteredServiceCounter()[typeof(ITestService)]);
            
        }

        [Test]
        public void two_module_registring_same_service()
        {
            _moduleLoader.LoadModuleFromFile(_pathToRegistering);
            Assert.AreEqual(1, ServiceRegistry.GetRegisteredServices().Count);
            Assert.Throws<Exception>(() => _moduleLoader.LoadModuleFromFile(_pathToRegistering));
        }

        [Test]
        public void module_demanding_service_loaded_without_registering_service_earlier()
        {
            Assert.Throws<Exception>( () => _moduleLoader.LoadModuleFromFile(_pathToResolving));
        }

    }
}