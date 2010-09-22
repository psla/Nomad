using System;

namespace Nomad.Exceptions
{
    /// <summary>
    ///     Exception raised when service is not found and no service can be provided.
    /// </summary>
    public class ServiceNotFoundException : Exception
    {
        /// <summary>
        ///     Initializes new instance of the <see cref="ServiceNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Message to be kept within exception.</param>
        public ServiceNotFoundException(string message) : base(message)
        {
            
        }

        /// <summary>
        ///     Initializes new instance of the <see cref="ServiceNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Message to be kept within exception.</param>
        /// <param name="innerException">Inner exception, causing this exception to be raised.</param>
        public ServiceNotFoundException(string message,Exception innerException) : base(message,innerException)
        {
            
        }
    }
}