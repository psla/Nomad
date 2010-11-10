using System;

namespace Nomad.Exceptions
{
    /// <summary>
    ///     Abstract class for all NomadExceptions.
    /// </summary>
    public abstract class NomadException : Exception
    {
        /// <summary>
        ///     Protected constructor used only for keeping inheritance lineage.
        /// </summary>
        /// <param name="message">Message to be kept within exception.</param>
        /// <param name="innerException">Inner exception to be kept within exception.</param>
        protected NomadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }


        /// <summary>
        ///     Protected constructor used only for keeping inheritance lineage.
        /// </summary>
        /// <param name="message">Message to be kept within exception.</param>
        protected NomadException(string message) : base(message)
        {
        }


        /// <summary>
        ///      Inherited <see cref="object.ToString"/> method which every exception has to implement.
        /// </summary>
        public abstract override string ToString();
    }
}