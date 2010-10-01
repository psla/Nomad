using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Moq;
using Nomad.Exceptions;
using Nomad.ServiceLocation;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.ServiceLocation
{
    [IntegrationTests]
    public class ServiceLocationIsolatedTests
    {
        #region Class and interfacces for test purposes

        public interface ITestInterface
        {
            int Execute();
        }

        #endregion

        private IServiceLocator _serviceLocator;


        [SetUp]
        public void SetUp()
        {
            var container = new WindsorContainer();
            _serviceLocator = new ServiceLocator(container);

            container.Register(
                Component.For<IServiceLocator>().Instance(_serviceLocator)
                );
        }


        [Test]
        public void resolve_returns_same_service_that_was_registered_for_given_interface()
        {
            //Prepare Mock for concrete implementation
            var mockServiceProvider = new Mock<ITestInterface>();

            _serviceLocator.Register(mockServiceProvider.Object);
            var serviceProvided = _serviceLocator.Resolve<ITestInterface>();

            Assert.AreSame(mockServiceProvider.Object, serviceProvided,
                           "Service (object) registered should be same instance as service resolved");
        }


        [Test]
        public void attempt_to_register_service_twice_results_in_exception()
        {
            var mockServiceProvider = new Mock<ITestInterface>();

            _serviceLocator.Register(mockServiceProvider.Object);

            Assert.Throws<DuplicateServiceException>(
                () => _serviceLocator.Register(mockServiceProvider.Object),
                "No exception thrown during second attempt to register already registered service");
        }


        [Test]
        public void
            when_registering_service_of_already_known_type_original_service_is_not_substituted()
        {
            var mockServiceProvider = new Mock<ITestInterface>();
            var mockServiceProvider2 = new Mock<ITestInterface>();

            _serviceLocator.Register(mockServiceProvider.Object);

            Assert.Throws<DuplicateServiceException>(
                () => _serviceLocator.Register(mockServiceProvider2.Object),
                "No exception thrown during second attempt to register already registered service");

            Assert.AreSame(mockServiceProvider.Object, _serviceLocator.Resolve<ITestInterface>(),
                           "Resolved service must be same as the first service registered");
        }


        [Test]
        public void attempt_to_resolve_unregistered_service_results_in_exception()
        {
            Assert.Throws<ServiceNotFoundException>(
                () => _serviceLocator.Resolve<ITestInterface>(),
                "The exception should be thrown during resolving unknown service");
        }


        [Test]
        public void attempt_to_register_null_service_results_in_exception()
        {
            ITestInterface serviceProvider = null;
            Assert.Throws<ArgumentNullException>(() => _serviceLocator.Register(serviceProvider),
                                                 "The exception should be thrown when passing null as service provider");
        }
    }
}