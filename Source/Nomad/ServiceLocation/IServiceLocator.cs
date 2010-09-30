using System;
using Nomad.Exceptions;

namespace Nomad.ServiceLocation
{
    /// <summary>
    ///     Provides means for registering and resolving Services of the specific interface.
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        ///     Registers passed object as an service implementation of interface T.
        /// </summary>
        /// <typeparam name="T">Interface of provided service.</typeparam>
        /// <param name="serviceProvider">Service implementation, thus service provider object.</param>
        /// <remarks>
        ///     Allows only One Service of the same interface to be registered at the time.
        /// </remarks>
        /// /// <exception cref="DuplicateServiceException">Raised during attempt to register same interface T as a service for second time.</exception>
        /// <exception cref="ArgumentNullException">Raised during attempt to pass null as service implantation</exception>
        void Register<T>(T serviceProvider);


        /// <summary>
        ///     Gets the object fulfilling the specified service interface.
        /// </summary>
        /// <typeparam name="T">Interface of the service type.</typeparam>
        /// <returns>Object implementing requested interface</returns>
        /// <exception cref="ServiceNotFoundException">Raised during attempt to resolve service which has not been registered before.</exception>
        T Resolve<T>();
    }
}