using System;
using Nomad.Modules.Manifest;

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
        /// <param name="message">Message to be passed with exception.</param>
        /// <param name="innerException">Inner exception which invoked this exception.</param>
        /// <param name="moduleName">The name of module which invoked this exception.</cs></param>
        public NomadCouldNotLoadModuleException(string message, Exception innerException,string  moduleName) : base(message,innerException)
        {
            ModuleName = moduleName;
        }

        /// <summary>
        ///     Initializes the instance of <see cref="NomadCouldNotLoadModuleException"/> class.
        /// </summary>
        /// <param name="message">Message to be passed with exception.</param>
        /// <param name="innerException">Inner exception which invoked this exception.</param>
        public NomadCouldNotLoadModuleException(string message,Exception innerException) : this(message,innerException,string.Empty)
        {
        }

        /// <summary>
        ///     Gets the manifest of the module which loading threw exception. 
        /// </summary>
        /// <remarks>
        ///     Might be <c>null</c> or <c>empty</c>.
        /// </remarks>
        public string ModuleName { get; private set; }
    }
}