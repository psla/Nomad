using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Moq;
using Nomad.ServiceLocation;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.ServiceLocation
{
    [UnitTests]
    public class ServiceLocationIsolatedTests
    {
        #region Class and interfacces for test purposes

        private interface ITestInterface
        {
            int Execute();
        }

        private class TestClass : ITestInterface
        {
            private readonly int _value;

            public TestClass(int value)
            {
                _value = value;
            }

            public int Execute()
            {
                return _value;
            }
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
        public void service_registration_service_resolving()
        {
            //int counter = 0;
            //Prepare Mock for concreate implementation
            //var mockServiceProvider = new Mock<ITestInterface>();
            //mockServiceProvider.Setup(x => x.Execute()).Callback(() => counter++);
            //_serviceLocator.Register<ITestInterface>(mockServiceProvider.Object);
            const int returnValue = 5;
            var serviceProvider = new TestClass(returnValue);
            _serviceLocator.Register<ITestInterface>(serviceProvider);

            var serviceProvided = _serviceLocator.Resolve<ITestInterface>();
            Assert.NotNull(serviceProvided);
            Assert.AreEqual(returnValue,serviceProvided.Execute());

            //behavioral check and state check
            //mockServiceProvider.Verify( x => x.Execute(), Times.Exactly(1));
            //Assert.AreEqual(1,counter);
        }

        [Test]
        public void double_same_service_regestration_expect_exception()
        {
            const int returnValue = 5;
            var serviceProvider = new TestClass(returnValue);

            _serviceLocator.Register<ITestInterface>(serviceProvider);

            Assert.Throws<ArgumentException>(() => _serviceLocator.Register<ITestInterface>(serviceProvider));
        }

        [Test]
        public void double_differnet_service_regestration_expect_fist_number_being_returned()
        {
            const int returnValue = 5;
            const int returnValue2 = 10;

            var serviceProvider = new TestClass(returnValue);
            _serviceLocator.Register<ITestInterface>(serviceProvider);

            var serviceProvider2 = new TestClass(returnValue2);
            Assert.Throws<ArgumentException>(() => _serviceLocator.Register<ITestInterface>(serviceProvider2));

            Assert.AreEqual(returnValue,_serviceLocator.Resolve<ITestInterface>().Execute());
        }

        [Test]
        public void no_regestration_before_resolving_service_expect_exception()
        {
            Assert.Throws<ArgumentException>(() => _serviceLocator.Resolve<ITestInterface>());
        }
    }
}