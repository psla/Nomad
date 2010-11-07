using System;

namespace Nomad.Exceptions
{
    /// <summary>
    ///     Occurs when module can not be loaded by framework.
    /// </summary>
    public class NomadCouldNotLoadModuleException : Exception
    {
        /// <summary>
        ///     Initializes the instance of <see cref="NomadCouldNotLoadModuleException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NomadCouldNotLoadModuleException(string message, Exception innerException) : base(message,innerException)
        {
            
        }

        //TODO: insert information about this exception
    }
}