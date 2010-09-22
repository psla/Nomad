using System;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Nomad.ServiceLocation
{
    /// <summary>
    ///     Default Nomad implementation of IServiceLocator based on Castle Windsor IoC Container.
    /// </summary>
    /// <remarks>
    ///     No clearing is performed for passed container. 
    /// </remarks>
    public class ServiceLocator : IServiceLocator
    {
        private readonly IWindsorContainer _serviceContainer;

        /// <summary>
        ///      Initializes new instance of the <see cref="ServiceLocator"/> class.
        /// </summary>
        /// <param name="windsorContainer">Passed container will be used as general container for all services.</param>
        public ServiceLocator(IWindsorContainer windsorContainer)
        {
            _serviceContainer = windsorContainer;   
        }

        #region Implementation of IServiceLocator

        /// <summary>
        ///     Registers the new interface T as a service provided by passed instance.
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <param name="serviceProvider">Concrete instance that provides the implementation of T</param>
        public void Register<T>(T serviceProvider)
        {
            bool found = true;

            var componentModel = new ComponentModel(serviceProvider.GetType().Name,typeof(T),serviceProvider.GetType());
            found = Component.ServiceAlreadyRegistered(_serviceContainer.Kernel,componentModel);

            if(found == false)
            {
                _serviceContainer.Register(
                Component.For<T>()
                    .Instance(serviceProvider)
                    .Unless(Component.ServiceAlreadyRegistered)
                );    
            }
            else
            {
                throw new ArgumentException("Service already registered");
            }
        }

        /// <summary>
        ///     Resolves the instance previously registered as service provider.
        /// </summary>
        /// <typeparam name="T">Interface of the service. </typeparam>
        /// <returns>Instance implementing T interface</returns>
        public T Resolve<T>()
        {
            try
            {
                return _serviceContainer.Resolve<T>();    
            }
            catch(Castle.MicroKernel.ComponentNotFoundException e)
            {
                throw new ArgumentException("Service not found",e); 
            }
            
        }

        #endregion
    }
}