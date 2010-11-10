using System;

namespace Nomad.Exceptions
{
    /// <summary>
    ///     Exception raised when service has already been registered.
    /// </summary>
    public class NomadDuplicateServiceException : NomadException
    {
        private readonly Type _serviceType;


        /// <summary>
        ///     Initializes new instance of the <see cref="NomadDuplicateServiceException"/> class.
        /// </summary>
        /// <param name="message">Message to be passed within exception.</param>
        /// <param name="serviceType">Type of service which is duplicated.</param>
        public NomadDuplicateServiceException(Type serviceType, string message) : base(message)
        {
            _serviceType = serviceType;
        }


        /// <summary>
        ///     Initializes new instance of the <see cref="NomadDuplicateServiceException"/> class.
        /// </summary>
        /// <param name="serviceType">Type of service which is duplicated.</param>
        /// <param name="message">Message to be passed within exception.</param>
        /// <param name="innerException">Inner exception, causing this exception to be raised.</param>
        public NomadDuplicateServiceException(Type serviceType, string message,
                                              Exception innerException)
            : base(message, innerException)
        {
            _serviceType = serviceType;
        }


        ///<summary>
        ///     Gets the type of the service that was marked as a duplicate.
        ///</summary>
        public Type ServiceType
        {
            get { return _serviceType; }
        }


        public override string ToString()
        {
            return
                string.Format(
                    "Could not register new service cause service of the same type {0} already exists. StackTrace: {1}",
                    ServiceType.FullName,
                    InnerException != null ? InnerException.StackTrace : string.Empty);
        }
    }
}