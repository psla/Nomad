using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Nomad.Exceptions;

namespace Nomad.Communication.ServiceLocation
{
    /// <summary>
    ///     Default Nomad implementation of IServiceLocator based on Castle Windsor IoC Container.
    /// </summary>
    public class ServiceLocator : IServiceLocator
    {
        private readonly IWindsorContainer _serviceContainer;


        /// <summary>
        ///      Initializes new instance of the <see cref="ServiceLocator"/> class.
        /// </summary>
        /// <param name="windsorContainer">Passed container will be used as container for all services.</param>
        public ServiceLocator(IWindsorContainer windsorContainer)
        {
            _serviceContainer = windsorContainer;
        }

        #region Implementation of IServiceLocator

        /// <summary>
        ///     Registers passed object as an service implementation of interface T.
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <param name="serviceProvider">Concrete instance that provides the implementation of T</param>
        /// <exception cref="DuplicateServiceException">Raised during attempt to register same interface T as a service for second time.</exception>
        /// <exception cref="ArgumentNullException">Raised during attempt to pass null as service implantation</exception>
        public void Register<T>(T serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            if (_serviceContainer.Kernel.HasComponent(typeof (T)))
                throw new DuplicateServiceException(typeof (T), "Service already registered");

            _serviceContainer.Register(
                Component.For<T>().Instance(serviceProvider)
                );
        }


        /// <summary>
        ///     Resolves the instance previously registered as service provider.
        /// </summary>
        /// <typeparam name="T">Interface of the service. </typeparam>
        /// <returns>Instance implementing T interface</returns>
        /// <exception cref="ServiceNotFoundException">Raised during attempt to resolve service which has not been registered before.</exception>
        public T Resolve<T>()
        {
            try
            {
                return _serviceContainer.Resolve<T>();
            }
            catch (ComponentNotFoundException e)
            {
                throw new ServiceNotFoundException(typeof (T), "Service not found", e);
            }
        }

        #endregion
    }
}