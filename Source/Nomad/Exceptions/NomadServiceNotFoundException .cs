using System;

namespace Nomad.Exceptions
{
    /// <summary>
    ///     Exception raised when service is not found and no service can be provided.
    /// </summary>
    public class NomadServiceNotFoundException : NomadException
    {
        private readonly Type _serviceType;


        /// <summary>
        ///     Initializes new instance of the <see cref="NomadServiceNotFoundException"/> class.
        /// </summary>
        /// <param name="serviceType">Type of service which was not found.</param>
        /// <param name="message">Message to be kept within exception.</param>
        public NomadServiceNotFoundException(Type serviceType, string message) : base(message)
        {
            _serviceType = serviceType;
        }


        /// <summary>
        ///     Initializes new instance of the <see cref="NomadServiceNotFoundException"/> class.
        /// </summary>
        /// <param name="serviceType">Type of service which was not found.</param>
        /// <param name="message">Message to be kept within exception.</param>
        /// <param name="innerException">Inner exception, causing this exception to be raised.</param>
        public NomadServiceNotFoundException(Type serviceType, string message,
                                             Exception innerException)
            : base(message, innerException)
        {
            _serviceType = serviceType;
        }


        /// <summary>
        ///     Gets the type of the service that was marked as an unknown.
        /// </summary>
        public Type ServiceType
        {
            get { return _serviceType; }
        }


        /// <summary>
        ///     Inherited.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Service {0} provided could not be found. StackTrace: {1}",
                                 ServiceType.FullName,
                                 InnerException != null ? InnerException.StackTrace : string.Empty);
        }
    }
}