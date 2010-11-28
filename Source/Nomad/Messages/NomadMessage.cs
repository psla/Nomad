using System;

namespace Nomad.Messages
{
    /// <summary>
    ///     Abstract class that defines the message passed between modules or kernel.
    /// </summary>
    /// <remarks>
    ///     This class has to be serializable.
    /// </remarks>
    [Serializable]
    public abstract class NomadMessage
    {
        /// <summary>
        ///     Text message. About cause of the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        ///     Protected constructor used only for inheritance lineage.
        /// </summary>
        /// <param name="message">Message to me passed with <see cref="NomadMessage"/> object. </param>
        protected NomadMessage(string message)
        {
            Message = message;
        }


        /// <summary>
        ///     Inherited <see cref="object.ToString"/> method which every message has to implement.
        /// </summary>
        public abstract override string ToString();
    }
}