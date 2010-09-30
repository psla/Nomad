using System;

namespace Nomad.Exceptions
{
    /// <summary>
    ///     Exception raised when service has already been registered.
    /// </summary>
    public class DuplicateServiceException : Exception
    {
        private readonly Type _serviceType;


        /// <summary>
        ///     Initializes new instance of the <see cref="DuplicateServiceException"/> class.
        /// </summary>
        /// <param name="message">Message to be passed within exception.</param>
        /// <param name="serviceType">Type of service which is duplicated.</param>
        public DuplicateServiceException(Type serviceType, string message) : base(message)
        {
            _serviceType = serviceType;
        }


        /// <summary>
        ///     Initializes new instance of the <see cref="DuplicateServiceException"/> class.
        /// </summary>
        /// <param name="serviceType">Type of service which is duplicated.</param>
        /// <param name="message">Message to be passed within exception.</param>
        /// <param name="innerException">Inner exception, causing this exception to be raised.</param>
        public DuplicateServiceException(Type serviceType, string message, Exception innerException)
            : base(message, innerException)
        {
            _serviceType = serviceType;
        }


        ///<summary>
        /// Gets the type of the service that was marked as a duplicate.
        ///</summary>
        public Type ServiceType
        {
            get { return _serviceType; }
        }
    }
}