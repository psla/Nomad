using Castle.MicroKernel.Registration;
using Nomad.Communication.ServiceLocation;
using Nomad.Tests.FunctionalTests.Modules;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.ServiceLocation
{
    [FunctionalTests]
    public class ServiceLocatorInModules : ModuleLoadingTestFixture
    {
        private const string PathToRegistering = @"Modules\ServiceLocator-Service";
        private const string PathToResolving = @"Modules\ServiceLocator-Client";

        private IServiceLocator _serviceLocator;


        [SetUp]
        public void SetUp()
        {
            ServiceRegistry.Clear();

            _serviceLocator = new ServiceLocator(Container);

            Container.Register(
                Component.For<IServiceLocator>().Instance(_serviceLocator)
                );
        }


        [Test]
        public void when_one_module_registers_a_service_other_module_can_resolve_it()
        {
            LoadModulesFromDirectory(PathToRegistering);

            Assert.AreEqual(1, ServiceRegistry.GetRegisteredServices().Count,
                            "Module has not called the constructor of the service provider class");
            Assert.DoesNotThrow(() => _serviceLocator.Resolve<ITestService>(),
                                "Module has not properly registered service provider");
            Assert.AreEqual(0, ServiceRegistry.GetRegisteredServiceCounter()[typeof (ITestService)],
                            "Method in service has already been used");


            LoadModulesFromDirectory(PathToResolving);

            Assert.AreEqual(1, ServiceRegistry.GetRegisteredServiceCounter()[typeof (ITestService)],
                            "Method in service was not called exactly one time");
        }


        // TODO: test defining what will happen if two modules register same service
        // TODO: test defining what will happen if module tries to resolve non existing service
    }
}