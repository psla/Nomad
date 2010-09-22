using System;

namespace Nomad.Exceptions
{
    /// <summary>
    ///     Exception raised when service has already been registered.
    /// </summary>
    public class DuplicateServiceException : Exception
    {
        /// <summary>
        ///     Initializes new instance of the <see cref="DuplicateServiceException"/> class.
        /// </summary>
        /// <param name="message">Message to be passed within exception.</param>
        public DuplicateServiceException(string message) : base(message)
        {
            
        }

        /// <summary>
        ///     Initializes new instance of the <see cref="DuplicateServiceException"/> class.
        /// </summary>
        /// <param name="message">Message to be passed within exception.</param>
        /// <param name="innerException">Inner exception, causing this exception to be raised.</param>
        public DuplicateServiceException(string message,Exception innerException) : base(message,innerException)
        {
            
        }
    }
}